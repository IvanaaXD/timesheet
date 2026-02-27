using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Country;

namespace TimeSheet.Application.Abstractions
{
    public interface ICountryService
    {
        Task<CountryDTO> GetCountryByIdAsync(Guid id);
        Task<CountryDTO> GetCountryByNameAsync(string name);
        Task<IEnumerable<CountryDTO>> GetAllCountriesAsync();
        Task<CountryDTO> CreateCountryAsync(CountryRequestDTO request);
        Task<CountryDTO> UpdateCountryAsync(Guid id, CountryRequestDTO request);
    }
}
