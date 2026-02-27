using System;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IProjectLeadRepository
    {
        Task AssignLeadAsync(Guid projectId, Guid memberId);
        Task RemoveLeadAsync(Guid projectId, Guid memberId);
        Task<IEnumerable<Project>> FindProjectsByLeadAsync(Guid memberId);
        Task<IEnumerable<Member>> FindLeadsByProjectAsync(Guid projectId);
        Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId);
    }
}
