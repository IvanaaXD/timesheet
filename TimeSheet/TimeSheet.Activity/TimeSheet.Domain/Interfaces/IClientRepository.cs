using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> FindClientByIdAsync(Guid id);
        Task<Client> FindClientByNameAsync(string name);
        Task<IEnumerable<Client>> FindAllClientsAsync();
        Task<Client> AddClientAsync(Client client);
        Task<Client> UpdateClientAsync(Client client);
        Task DeleteClientAsync(Client client);
        Task<PagedList<Client>> FindAllClientsPagedAsync(
                int pageNumber,
                int pageSize,
                string? searchTerm,
                string? firstLetter,
                string order);           
    }
}
