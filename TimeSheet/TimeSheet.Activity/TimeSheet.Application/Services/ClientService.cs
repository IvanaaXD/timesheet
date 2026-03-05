using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Client;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Application.Common.DTOs;
using TimeSheet.Application.Common.Exceptions;
using FluentValidation;
using TimeSheet.Application.Validators;
using TimeSheet.Application.Common.Extensions;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IValidator<ClientRequestDTO> _validator;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepository, ICountryRepository countryRepository, IValidator<ClientRequestDTO> validator, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _countryRepository = countryRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ClientDTO> GetClientByIdAsync(Guid id)
        {
            var client = await _clientRepository.FindClientByIdAsync(id);
            if (client == null) throw new NotFoundException($"Client with ID {id} not found.");

            return _mapper.Map<ClientDTO>(client);
        }

        public async Task<ClientDTO> GetClientByNameAsync(string name)
        {
            var client = await _clientRepository.FindClientByNameAsync(name);
            if (client == null) throw new NotFoundException($"Client with name {name} not found.");

            return _mapper.Map<ClientDTO>(client);
        }

        public async Task<IEnumerable<ClientDTO>> GetAllClientsAsync()
        {
            var clients = await _clientRepository.FindAllClientsAsync();
            return _mapper.Map<IEnumerable<ClientDTO>>(clients);
        }

        public async Task<PagedList<ClientDTO>> GetAllClientsPagedAsync(PagedListDTO pagedListDTO)
        {
            var pagedClients = await _clientRepository.FindAllClientsPagedAsync(
                pagedListDTO.PageNumber, pagedListDTO.PageSize, pagedListDTO.SearchTerm, pagedListDTO.FirstLetter, pagedListDTO.Order);

            var dtos = _mapper.Map<PagedList<ClientDTO>>(pagedClients);

            return dtos;
        }

        public async Task<ClientDTO> CreateClientAsync(ClientRequestDTO clientRequestDTO)
        {
            await ValidateAndThrowAsync(_validator, clientRequestDTO);

            var existing = await _clientRepository.FindClientByNameAsync(clientRequestDTO.Name);
            if (existing != null)
                throw new ConflictException($"Client with name '{clientRequestDTO.Name}' already exists.");

            var country = await _countryRepository.FindCountryByIdAsync(clientRequestDTO.CountryId);
            if (country == null)
                throw new NotFoundException($"Country with ID {clientRequestDTO.CountryId} not found.");

            var client = _mapper.Map<Client>(clientRequestDTO);

            var createdClient = await _clientRepository.AddClientAsync(client);
            return _mapper.Map<ClientDTO>(createdClient);
        }

        public async Task<ClientDTO> UpdateClientAsync(Guid id, ClientRequestDTO clientRequestDTO)
        {
            await ValidateAndThrowAsync(_validator, clientRequestDTO);

            var existingClient = await _clientRepository.FindClientByIdAsync(id);
            if (existingClient == null) throw new NotFoundException($"Client with ID {id} not found.");

            var clientWithSameName = await _clientRepository.FindClientByNameAsync(clientRequestDTO.Name);
            if (clientWithSameName != null && clientWithSameName.Id != id)
                throw new ConflictException($"Another client already has the name '{clientRequestDTO.Name}'.");

            var country = await _countryRepository.FindCountryByIdAsync(clientRequestDTO.CountryId);
            if (country == null)
                throw new NotFoundException($"Country with ID {clientRequestDTO.CountryId} not found.");

            _mapper.Map(clientRequestDTO, existingClient);
            var updatedClient = await _clientRepository.UpdateClientAsync(existingClient);
            return _mapper.Map<ClientDTO>(updatedClient);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            var existingClient = await _clientRepository.FindClientByIdAsync(id);
            if (existingClient == null) throw new NotFoundException($"Client with ID {id} not found.");

            await _clientRepository.DeleteClientAsync(existingClient);
        }
    }
}