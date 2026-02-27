using System;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> FindClientByIdAsync(Guid id);
        Task<Client> FindClientByNameAsync(string name);
        Task<IEnumerable<Client>> FindAllClientsAsync();
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(Client client);
        Task<PagedList<Client>> FindAllClientsPagedAsync(
                int pageNumber,
                int pageSize,
                string? searchTerm,
                string? firstLetter,
                string order);           
    }
}
