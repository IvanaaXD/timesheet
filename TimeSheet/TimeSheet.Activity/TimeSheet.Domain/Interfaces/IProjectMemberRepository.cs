using System;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IProjectMemberRepository
    {
        Task<IEnumerable<Member>> FindMembersByProjectAsync(Guid projectId);
        Task<IEnumerable<Project>> FindProjectsByMemberAsync(Guid memberId);
        Task<bool> IsMemberOnProjectAsync(Guid memberId, Guid projectId);
        Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId);
        Task AddMemberToProjectAsync(Guid projectId, Guid memberId, bool isLead);
        Task ChangeMemberRoleOnProjectAsync(Guid projectId, Guid memberId, bool isLead);
        Task RemoveMemberFromProjectAsync(Guid projectId, Guid memberId);
    }
}
