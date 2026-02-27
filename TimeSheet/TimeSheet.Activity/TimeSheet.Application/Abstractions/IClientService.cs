using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Application.DTOs.Client;
using TimeSheet.Application.Common.DTOs;

namespace TimeSheet.Application.Abstractions
{
    public interface IClientService
    {
        Task<ClientDTO> GetClientByIdAsync(Guid id);
        Task<ClientDTO> GetClientByNameAsync(string name);
        Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
        Task<ClientDTO> CreateClientAsync(ClientRequestDTO request);
        Task<ClientDTO> UpdateClientAsync(Guid id, ClientRequestDTO request);
        Task DeleteClientAsync(Guid id);
        Task<PagedList<ClientDTO>> GetAllClientsPagedAsync(PagedListDTO pagedListDTO);
    }
}
