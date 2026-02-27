using System;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country> FindCountryByIdAsync(Guid id);
        Task<Country> FindCountryByNameAsync(string name);
        Task<IEnumerable<Country>> FindAllCountriesAsync();
        Task AddCountryAsync(Country country);
        Task UpdateCountryAsync(Country country);
    }
}
