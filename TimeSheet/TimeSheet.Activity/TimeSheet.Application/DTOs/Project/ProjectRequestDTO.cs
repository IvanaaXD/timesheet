using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities.Enums;

namespace TimeSheet.Application.DTOs.Project
{
    public class ProjectRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectStatus Status { get; set; }

        // Relationships

        public Guid ClientId { get; set; }
        public Guid? CurrentLead { get; set; }

        // Constructors

        public ProjectRequestDTO() { }
        public ProjectRequestDTO(string name, string description, ProjectStatus status)
        {
            Name = name;
            Description = description;
            Status = status;
        }
    }
}
