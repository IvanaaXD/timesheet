using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities.Enums;

namespace TimeSheet.Application.DTOs.Project
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectStatus Status { get; set; }

        // Relationships

        public string ClientName { get; set; }
        public string? CurrentLeadName { get; set; }
        public IEnumerable<string> TeamMembers { get; set; } = new List<string>();

        // Constructors

        public ProjectDTO() { }
        public ProjectDTO(string name, string description, ProjectStatus status)
        {
            Name = name;
            Description = description;
            Status = status;
        }
    }
}
