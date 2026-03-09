using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TimeSheetDbContext _context;

        public ProjectRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        private IQueryable<Project> GetProjectsWithIncludes()
        {
            return _context.Projects
                .Include(p => p.Client)
                .Include(p => p.CurrentLead)
                .Include(p => p.TeamMembers) 
                    .ThenInclude(tm => tm.Member) 
                .AsQueryable();
        }

        public async Task<Project?> FindProjectByIdAsync(Guid id)
        {
            return await GetProjectsWithIncludes().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> FindAllProjectsAsync()
        {
            return await GetProjectsWithIncludes().AsNoTracking().ToListAsync();
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task DeleteProjectAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<Project>> FindAllProjectsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? firstLetter,
            string order)
        {
            var query = GetProjectsWithIncludes().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(firstLetter))
            {
                query = query.Where(c => c.Name.ToLower().StartsWith(firstLetter.ToLower()));
            }

            query = order.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedList<Project>(items, totalCount, pageNumber, pageSize, firstLetter);
        }
    }
}
