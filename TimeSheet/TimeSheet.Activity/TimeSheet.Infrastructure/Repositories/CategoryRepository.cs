using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TimeSheetDbContext _context;

        public CategoryRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task<Category> FindCategoryByIdAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> FindCategoryByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(x  => x.Name == name);
        }

        public async Task<IEnumerable<Category>> FindAllCategoriesAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _context.SaveChangesAsync();
        }
    }
}
