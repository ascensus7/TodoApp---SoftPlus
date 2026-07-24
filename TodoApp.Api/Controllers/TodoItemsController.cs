using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Extensions;
using TodoApp.Interfaces.Common;
using TodoApp.Interfaces.DTOs.Tasks;
using TodoApp.Interfaces.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/todo-items")]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;

    public TodoItemsController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    // GET: /api/todo-items?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<TodoItemDto>>> GetAll(
        [FromQuery] TodoItemQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await _todoItemService.GetAllAsync(
            queryParameters,
            userId,
            cancellationToken);

        return Ok(result);
    }

    // GET: /api/todo-items/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItemDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var todoItem = await _todoItemService.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        return Ok(todoItem);
    }

    // POST: /api/todo-items
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> Create(
        CreateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var todoItem = await _todoItemService.CreateAsync(
            request,
            userId,
            cancellationToken);

        // Returns 201 Created and the URL of the created task.
        return CreatedAtAction(
            nameof(GetById),
            new { id = todoItem.Id },
            todoItem);
    }

    // PUT: /api/todo-items/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItemDto>> Update(
        int id,
        UpdateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var todoItem = await _todoItemService.UpdateAsync(
            id,
            request,
            userId,
            cancellationToken);

        return Ok(todoItem);
    }

    // DELETE: /api/todo-items/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _todoItemService.DeleteAsync(
            id,
            userId,
            cancellationToken);

        return NoContent();
    }
}