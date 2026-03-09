using System;

namespace TimeSheet.Application.DTOs.ProjectMember
{
    public class ProjectMemberRequestDTO
    {
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }
        public bool IsLead { get; set; } = false;
    }
}