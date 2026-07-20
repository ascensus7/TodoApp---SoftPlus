using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.Interfaces.DTOs.Categories;

namespace TodoApp.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(string userId, CancellationToken cancellationToken = default);

        Task<CategoryDto> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);

        Task<CategoryDto> CreateAsync(CreateCategoryRequest request, string userId, CancellationToken cancellationToken = default);

        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, string userId, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    }
}
