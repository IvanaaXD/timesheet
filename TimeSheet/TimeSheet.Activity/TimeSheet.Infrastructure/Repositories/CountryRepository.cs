using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly TimeSheetDbContext _context;

        public CountryRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task<Country> FindCountryByIdAsync(Guid id)
        {
            return await _context.Countries.FindAsync(id);
        }

        public async Task<Country> FindCountryByNameAsync(string name)
        {
            return await _context.Countries.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<IEnumerable<Country>> FindAllCountriesAsync()
        {
            return await _context.Countries.AsNoTracking().ToListAsync();
        }

        public async Task AddCountryAsync(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCountryAsync(Country country)
        {
            await _context.SaveChangesAsync();
        }
    }
}
