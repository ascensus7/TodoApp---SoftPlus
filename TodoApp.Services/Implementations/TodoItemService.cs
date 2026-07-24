using Microsoft.EntityFrameworkCore;
using TodoApp.DataAccess.Context;
using TodoApp.Domain.Entities;
using TodoApp.Interfaces.Common;
using TodoApp.Interfaces.DTOs.Tasks;
using TodoApp.Interfaces.Services;
using TodoApp.Services.Exceptions;

namespace TodoApp.Services.Implementations;

public class TodoItemService : ITodoItemService
{
    private readonly AppDbContext _dbContext;

    public TodoItemService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TodoItemDto>> GetAllAsync(
        TodoItemQueryParameters queryParameters,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Start with tasks belonging to the current user.
        IQueryable<TodoItem> query = _dbContext.TodoItems
            .AsNoTracking()
            .Where(todoItem => todoItem.UserId == userId);

        // Apply case-insensitive search to title and description.
        if (!string.IsNullOrWhiteSpace(queryParameters.Search))
        {
            var searchPattern = $"%{queryParameters.Search.Trim()}%";

            query = query.Where(todoItem =>
                EF.Functions.ILike(todoItem.Title, searchPattern) ||
                (todoItem.Description != null &&
                 EF.Functions.ILike(todoItem.Description, searchPattern)));
        }

        // Apply category filter only when CategoryId was provided.
        if (queryParameters.CategoryId.HasValue)
        {
            query = query.Where(todoItem =>
                todoItem.CategoryId == queryParameters.CategoryId.Value);
        }

        // Apply completion filter only when IsCompleted was provided.
        if (queryParameters.IsCompleted.HasValue)
        {
            query = query.Where(todoItem =>
                todoItem.IsCompleted == queryParameters.IsCompleted.Value);
        }

        // Count all matching tasks before applying pagination.
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply ordering and pagination inside the database.
        var items = await query
            .OrderByDescending(todoItem => todoItem.CreatedAt)
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(todoItem => new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                UpdatedAt = todoItem.UpdatedAt,
                DueDate = todoItem.DueDate,
                CategoryId = todoItem.CategoryId,

                // EF Core creates the required JOIN automatically.
                CategoryName = todoItem.Category == null
                    ? null
                    : todoItem.Category.Name
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TodoItemDto>
        {
            Items = items,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TodoItemDto> GetByIdAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Select by both task ID and owner ID.
        var todoItem = await _dbContext.TodoItems
            .AsNoTracking()
            .Where(todoItem => todoItem.Id == id && todoItem.UserId == userId)
            .Select(todoItem => new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                UpdatedAt = todoItem.UpdatedAt,
                DueDate = todoItem.DueDate,
                CategoryId = todoItem.CategoryId,
                CategoryName = todoItem.Category == null
                    ? null
                    : todoItem.Category.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (todoItem == null)
        {
            throw new NotFoundException($"Todo item with ID {id} was not found.");
        }

        return todoItem;
    }

    public async Task<TodoItemDto> CreateAsync(
        CreateTodoItemRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var title = NormalizeTitle(request.Title);

        // Also verifies that the category belongs to this user.
        var categoryName = await GetCategoryNameAsync(
            request.CategoryId,
            userId,
            cancellationToken);

        var todoItem = new TodoItem
        {
            Title = title,
            Description = NormalizeOptionalText(request.Description),
            IsCompleted = false,
            CreatedAt = DateTimeOffset.UtcNow,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            UserId = userId
        };

        _dbContext.TodoItems.Add(todoItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(todoItem, categoryName);
    }

    public async Task<TodoItemDto> UpdateAsync(
        int id,
        UpdateTodoItemRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Tracking is enabled because this entity will be updated.
        var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(
            todoItem => todoItem.Id == id && todoItem.UserId == userId,
            cancellationToken);

        if (todoItem == null)
        {
            throw new NotFoundException($"Todo item with ID {id} was not found.");
        }

        var title = NormalizeTitle(request.Title);

        // Validate the new category before changing the entity.
        var categoryName = await GetCategoryNameAsync(
            request.CategoryId,
            userId,
            cancellationToken);

        todoItem.Title = title;
        todoItem.Description = NormalizeOptionalText(request.Description);
        todoItem.IsCompleted = request.IsCompleted;
        todoItem.DueDate = request.DueDate;
        todoItem.CategoryId = request.CategoryId;
        todoItem.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(todoItem, categoryName);
    }

    public async Task DeleteAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(
            todoItem => todoItem.Id == id && todoItem.UserId == userId,
            cancellationToken);

        if (todoItem == null)
        {
            throw new NotFoundException($"Todo item with ID {id} was not found.");
        }

        _dbContext.TodoItems.Remove(todoItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<string?> GetCategoryNameAsync(
        int? categoryId,
        string userId,
        CancellationToken cancellationToken)
    {
        // A task is allowed to have no category.
        if (!categoryId.HasValue)
        {
            return null;
        }

        var categoryName = await _dbContext.Categories
            .AsNoTracking()
            .Where(category =>
                category.Id == categoryId.Value &&
                category.UserId == userId)
            .Select(category => category.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (categoryName == null)
        {
            throw new NotFoundException(
                $"Category with ID {categoryId.Value} was not found.");
        }

        return categoryName;
    }

    private static string NormalizeTitle(string title)
    {
        var normalizedTitle = title.Trim();

        // Required does not reject strings containing only spaces.
        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            throw new ArgumentException("Todo item title cannot be empty.");
        }

        return normalizedTitle;
    }

    private static string? NormalizeOptionalText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text.Trim();
    }

    private static TodoItemDto MapToDto(
        TodoItem todoItem,
        string? categoryName)
    {
        return new TodoItemDto
        {
            Id = todoItem.Id,
            Title = todoItem.Title,
            Description = todoItem.Description,
            IsCompleted = todoItem.IsCompleted,
            CreatedAt = todoItem.CreatedAt,
            UpdatedAt = todoItem.UpdatedAt,
            DueDate = todoItem.DueDate,
            CategoryId = todoItem.CategoryId,
            CategoryName = categoryName
        };
    }
}