using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Extensions;
using TodoApp.Interfaces.DTOs.Categories;
using TodoApp.Interfaces.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: /api/categories
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var categories = await _categoryService.GetAllAsync(
            userId,
            cancellationToken);

        return Ok(categories);
    }

    // GET: /api/categories/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var category = await _categoryService.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        return Ok(category);
    }

    // POST: /api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var category = await _categoryService.CreateAsync(
            request,
            userId,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            category);
    }

    // PUT: /api/categories/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(
        int id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var category = await _categoryService.UpdateAsync(
            id,
            request,
            userId,
            cancellationToken);

        return Ok(category);
    }

    // DELETE: /api/categories/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _categoryService.DeleteAsync(
            id,
            userId,
            cancellationToken);

        return NoContent();
    }
}