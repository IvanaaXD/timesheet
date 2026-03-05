using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Country;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.Common.Exceptions;
using FluentValidation;
using TimeSheet.Application.Validators;
using TimeSheet.Application.Common.Extensions;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IValidator<CountryRequestDTO> _validator;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository countryRepository, IValidator<CountryRequestDTO> validator, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _validator = validator;
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
            await _validator.ValidateAndThrowAsync(countryRequestDTO);

            var countryWithSameName = await _countryRepository.FindCountryByNameAsync(countryRequestDTO.Name);
            if (countryWithSameName != null)
                throw new ConflictException($"Country with name '{countryRequestDTO.Name}' already exists.");

            var country = _mapper.Map<Country>(countryRequestDTO);

            var createdCountry = await _countryRepository.AddCountryAsync(country);
            return _mapper.Map<CountryDTO>(createdCountry);
        }

        public async Task<CountryDTO> UpdateCountryAsync(Guid id, CountryRequestDTO countryRequestDTO)
        {
            await _validator.ValidateAndThrowAsync(countryRequestDTO);

            var existingCountry = await _countryRepository.FindCountryByIdAsync(id);
            if (existingCountry == null) throw new NotFoundException($"Country with ID {id} not found.");

            var countryWithSameName = await _countryRepository.FindCountryByNameAsync(countryRequestDTO.Name);
            if (countryWithSameName != null && countryWithSameName.Id != id)
            {
                throw new ConflictException($"Another country already has the name '{countryRequestDTO.Name}'.");
            }

            _mapper.Map(countryRequestDTO, existingCountry);

            var updatedCountry = await _countryRepository.UpdateCountryAsync(existingCountry);
            return _mapper.Map<CountryDTO>(updatedCountry);
        }
    }
}
