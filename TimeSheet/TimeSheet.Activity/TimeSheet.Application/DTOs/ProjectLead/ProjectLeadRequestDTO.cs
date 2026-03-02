using System;

namespace TimeSheet.Application.DTOs.ProjectLead
{
    public class ProjectLeadRequestDTO
    {
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }
    }
}