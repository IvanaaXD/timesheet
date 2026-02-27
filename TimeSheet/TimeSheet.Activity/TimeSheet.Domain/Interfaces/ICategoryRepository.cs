using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> FindCategoryByIdAsync(Guid id);
        Task<Category> FindCategoryByNameAsync(string name);
        Task<IEnumerable<Category>> FindAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
    }
}
