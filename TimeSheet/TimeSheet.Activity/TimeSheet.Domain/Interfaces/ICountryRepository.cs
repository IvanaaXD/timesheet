using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country> FindCountryByIdAsync(Guid id);
        Task<Country> FindCountryByNameAsync(string name);
        Task<IEnumerable<Country>> FindAllCountriesAsync();
        Task<Country> AddCountryAsync(Country country);
        Task<Country> UpdateCountryAsync(Country country);
    }
}
