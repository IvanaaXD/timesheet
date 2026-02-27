using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly TimeSheetDbContext _context;

        public ClientRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task<Client> FindClientByIdAsync(Guid id)
        {
            return await _context.Clients.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> FindClientByNameAsync(string name)
        {
            return await _context.Clients.Include(c => c.Country).FirstOrDefaultAsync(x  => x.Name == name);
        }

        public async Task<IEnumerable<Client>> FindAllClientsAsync()
        {
            return await _context.Clients.Include(c => c.Country).AsNoTracking().ToListAsync();
        }

        public async Task AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientAsync(Client client)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(Client client)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<Client>> FindAllClientsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? firstLetter,
            string order)
        {
            var query = _context.Clients.Include(c => c.Country).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(firstLetter))
            {
                query = query.Where(c => c.Name.StartsWith(firstLetter));
            }

            query = order.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedList<Client>(items, totalCount, pageNumber, pageSize);
        }
    }
}
