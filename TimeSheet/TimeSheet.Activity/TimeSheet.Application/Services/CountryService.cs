using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Country;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.Exceptions;

namespace TimeSheet.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        public async Task<CountryDTO> GetCountryByIdAsync(Guid id)
        {
            var country = await _countryRepository.FindCountryByIdAsync(id);
            if (country == null) throw new NotFoundException($"Country with ID {id} not found.");

            return _mapper.Map<CountryDTO>(country);
        }

        public async Task<CountryDTO> GetCountryByNameAsync(string name)
        {
            var country = await _countryRepository.FindCountryByNameAsync(name);
            if (country == null) throw new NotFoundException($"Country with name {name} not found.");

            return _mapper.Map<CountryDTO>(country);
        }

        public async Task<IEnumerable<CountryDTO>> GetAllCountriesAsync()
        {
            var countries = await _countryRepository.FindAllCountriesAsync();
            return _mapper.Map<IEnumerable<CountryDTO>>(countries);
        }

        public async Task<CountryDTO> CreateCountryAsync(CountryRequestDTO countryRequestDTO)
        {
            var country = _mapper.Map<Country>(countryRequestDTO);
            country.Id = Guid.NewGuid();

            await _countryRepository.AddCountryAsync(country);
            return _mapper.Map<CountryDTO>(country);
        }

        public async Task<CountryDTO> UpdateCountryAsync(Guid id, CountryRequestDTO countryRequestDTO)
        {
            var existingCountry = await _countryRepository.FindCountryByIdAsync(id);
            if (existingCountry == null) throw new NotFoundException($"Country with ID {id} not found.");

            _mapper.Map(countryRequestDTO, existingCountry);

            await _countryRepository.UpdateCountryAsync(existingCountry);
            return _mapper.Map<CountryDTO>(existingCountry);
        }
    }
}
