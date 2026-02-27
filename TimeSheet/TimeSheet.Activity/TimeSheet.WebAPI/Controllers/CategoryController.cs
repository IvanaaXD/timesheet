using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Category;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            return Ok(result);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var result = await _categoryService.GetCategoryByNameAsync(name);

            if (result == null)
            {
                return NotFound(new { message = $"Category with ID {name} not found." });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDTO categoryRequestDTO)
        {
            var result = await _categoryService.CreateCategoryAsync(categoryRequestDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryRequestDTO categoryRequestDTO, Guid id)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryRequestDTO);
            return Ok(result);
        }
    }
}