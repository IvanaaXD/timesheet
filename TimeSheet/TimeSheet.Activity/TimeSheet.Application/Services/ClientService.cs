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

namespace TimeSheet.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<ClientDTO> GetClientByIdAsync(Guid id)
        {
            var client = await _clientRepository.FindClientByIdAsync(id);
            if (client == null) throw new KeyNotFoundException($"Client with ID {id} not found.");

            return _mapper.Map<ClientDTO>(client);
        }

        public async Task<ClientDTO> GetClientByNameAsync(string name)
        {
            var client = await _clientRepository.FindClientByNameAsync(name);
            if (client == null) throw new KeyNotFoundException($"Client with name {name} not found.");

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
            var client = _mapper.Map<Client>(clientRequestDTO);
            client.Id = Guid.NewGuid();

            await _clientRepository.AddClientAsync(client);

            var createdClient = await _clientRepository.FindClientByIdAsync(client.Id);
            return _mapper.Map<ClientDTO>(createdClient);
        }

        public async Task<ClientDTO> UpdateClientAsync(Guid id, ClientRequestDTO clientRequestDTO)
        {
            var existingClient = await _clientRepository.FindClientByIdAsync(id);
            if (existingClient == null) throw new KeyNotFoundException($"Client with ID {id} not found.");

            _mapper.Map(clientRequestDTO, existingClient);
            await _clientRepository.UpdateClientAsync(existingClient);

            var updatedClient = await _clientRepository.FindClientByIdAsync(id);
            return _mapper.Map<ClientDTO>(updatedClient);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            var existingClient = await _clientRepository.FindClientByIdAsync(id);
            if (existingClient == null) throw new KeyNotFoundException($"Client with ID {id} not found.");

            await _clientRepository.DeleteClientAsync(existingClient);
        }
    }
}