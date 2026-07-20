using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.DataAccess.Context;
using TodoApp.Domain.Entities;
using TodoApp.Interfaces.DTOs.Categories;
using TodoApp.Interfaces.Services;
using TodoApp.Services.Exceptions;

namespace TodoApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _dbContext;
        public CategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Implementation for getting all categories for a specific user
            return await _dbContext.Categories
           .AsNoTracking()
           .Where(category => category.UserId == userId)
           .OrderBy(category => category.Name)
           .Select(category => new CategoryDto
           {
               Id = category.Id,
               Name = category.Name
           })
           .ToListAsync(cancellationToken);
        }

        public async Task<CategoryDto> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                 .AsNoTracking()
                    .Where(category =>
                        category.Id == id &&
                        category.UserId == userId)
                    .Select(category => new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {id} not found.");
            }
            return category;
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, string userId, CancellationToken cancellationToken = default)
        {
            var name = request.Name?.Trim();
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }
            var normalizedName = name.ToLowerInvariant();
            var categoryExists = await _dbContext.Categories
                .AnyAsync(category => category.UserId == userId && category.NormalizedName == normalizedName, cancellationToken);
            if (categoryExists)
            {
                throw new ConflictException(
                    "A category with the same name already exists.");
            }
            var category = new Category
            {
                Name = name,
                NormalizedName = normalizedName,
                UserId = userId
            };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return MapToDto(category);

        }


        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, string userId, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {id} not found.");
            }
            var name = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }
            var normalizedName = name.ToLowerInvariant();
            var categoryExists = await _dbContext.Categories
                .AnyAsync(c => c.UserId == userId && c.NormalizedName == normalizedName && c.Id != id, cancellationToken);
            if (categoryExists)
            {
                throw new ConflictException("A category with the same name already exists.");
            }
            category.Name = name;
            category.NormalizedName = normalizedName;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return MapToDto(category);
        }


        public async Task DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {id} not found.");
            }
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

    }
}
