using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.DTOs.Project;

namespace TimeSheet.Application.Abstractions
{
    public interface IProjectLeadService
    {
        Task AssignLeadAsync(Guid projectId, Guid memberId);
        Task RemoveLeadAsync(Guid projectId, Guid memberId);
        Task<IEnumerable<ProjectDTO>> GetProjectsByLeadAsync(Guid memberId);
        Task<IEnumerable<MemberDTO>> GetLeadsByProjectAsync(Guid projectId);
        Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId);
    }
}
