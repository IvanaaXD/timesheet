using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.DTOs.Project;

namespace TimeSheet.Application.Abstractions
{
    public interface IProjectMemberService
    {
        Task AddMemberToProjectAsync(Guid projectId, Guid memberId, bool isLead);
        Task AssignLeadAsync(Guid projectId, Guid memberId);
        Task RemoveLeadAsync(Guid projectId, Guid memberId);
        Task RemoveMemberFromProjectAsync(Guid projectId, Guid memberId);
        Task<IEnumerable<ProjectDTO>> GetProjectsByMemberAsync(Guid memberId);
        Task<IEnumerable<MemberDTO>> GetMembersByProjectAsync(Guid projectId);
        Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId);
    }
}
