using TodoApp.Interfaces.Common;
using TodoApp.Interfaces.DTOs.Tasks;

namespace TodoApp.Interfaces.Services;

public interface ITodoItemService
{
    // Returns one filtered and paginated page of the user's tasks.
    Task<PagedResult<TodoItemDto>> GetAllAsync(
        TodoItemQueryParameters queryParameters,
        string userId,
        CancellationToken cancellationToken = default);

    // Returns one task only if it belongs to the current user.
    Task<TodoItemDto> GetByIdAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default);

    // Creates a task for the current user.
    Task<TodoItemDto> CreateAsync(
        CreateTodoItemRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    // Updates a task only if it belongs to the current user.
    Task<TodoItemDto> UpdateAsync(
        int id,
        UpdateTodoItemRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    // Deletes a task only if it belongs to the current user.
    Task DeleteAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default);
}