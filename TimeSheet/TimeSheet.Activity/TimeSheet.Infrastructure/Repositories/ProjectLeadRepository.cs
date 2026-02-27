using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class ProjectLeadRepository : IProjectLeadRepository
    {
        private readonly TimeSheetDbContext _context;

        public ProjectLeadRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task AssignLeadAsync(Guid projectId, Guid memberId)
        {
            var projectLead = new ProjectLead
            {
                ProjectId = projectId,
                MemberId = memberId
            };

            _context.ProjectLeads.Add(projectLead);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Member>> FindLeadsByProjectAsync(Guid projectId)
        {
            return await _context.ProjectLeads
                .Where(pl => pl.ProjectId == projectId)
                .Select(pl => pl.Member) 
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> FindProjectsByLeadAsync(Guid memberId)
        {
            return await _context.ProjectLeads
                .Where(pl => pl.MemberId == memberId)
                .Select(pl => pl.Project) 
                .ToListAsync();
        }

        public async Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId)
        {
            return await _context.ProjectLeads
                .AnyAsync(pl => pl.MemberId == memberId && pl.ProjectId == projectId);
        }

        public async Task RemoveLeadAsync(Guid projectId, Guid memberId)
        {
            var leadRecord = await _context.ProjectLeads
                .FirstOrDefaultAsync(pl => pl.ProjectId == projectId && pl.MemberId == memberId);

            if (leadRecord != null)
            {
                _context.ProjectLeads.Remove(leadRecord);
                await _context.SaveChangesAsync();
            }
        }
    }
}
