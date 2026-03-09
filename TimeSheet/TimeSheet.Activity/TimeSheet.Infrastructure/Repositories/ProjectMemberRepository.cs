using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly TimeSheetDbContext _context;

        public ProjectMemberRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> FindMembersByProjectAsync(Guid projectId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Select(pm => pm.Member)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> FindProjectsByMemberAsync(Guid memberId)
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.CurrentLead)
                .Where(p => _context.ProjectMembers
                    .Any(pm => pm.ProjectId == p.Id && pm.MemberId == memberId))
                .ToListAsync();
        }

        public async Task<bool> IsMemberOnProjectAsync(Guid memberId, Guid projectId)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.MemberId == memberId && pm.ProjectId == projectId);
        }

        public async Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.MemberId == memberId && pm.ProjectId == projectId && pm.IsLead == true);
        }

        public async Task AddMemberToProjectAsync(Guid projectId, Guid memberId, bool isLead)
        {
            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                MemberId = memberId,
                IsLead = isLead
            };

            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeMemberRoleOnProjectAsync(Guid projectId, Guid memberId, bool isLead)
        {
            var membershipRecord = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.MemberId == memberId);

            if (membershipRecord != null)
            {
                membershipRecord.IsLead = isLead;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveMemberFromProjectAsync(Guid projectId, Guid memberId)
        {
            var membershipRecord = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.MemberId == memberId);

            if (membershipRecord != null)
            {
                _context.ProjectMembers.Remove(membershipRecord);
                await _context.SaveChangesAsync();
            }
        }
    }
}
