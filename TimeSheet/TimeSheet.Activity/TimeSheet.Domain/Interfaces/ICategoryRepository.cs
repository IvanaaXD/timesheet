using System.Threading.Tasks;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> FindCategoryByIdAsync(Guid id);
        Task<Category> FindCategoryByNameAsync(string name);
        Task<IEnumerable<Category>> FindAllCategoriesAsync();
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
    }
}
