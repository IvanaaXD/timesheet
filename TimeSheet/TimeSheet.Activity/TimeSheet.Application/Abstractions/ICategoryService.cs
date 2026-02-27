using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Category;

namespace TimeSheet.Application.Abstractions
{
    public interface ICategoryService
    {
        Task<CategoryDTO> GetCategoryByIdAsync(Guid id);
        Task<CategoryDTO> GetCategoryByNameAsync(string name);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> CreateCategoryAsync(CategoryRequestDTO request);
        Task<CategoryDTO> UpdateCategoryAsync(Guid id, CategoryRequestDTO request);
    }
}
