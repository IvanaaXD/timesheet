using System;
using TimeSheet.Domain.Entities.Enums;
using TimeSheet.Domain.Interfaces;

namespace TimeSheet.Domain.Entities
{
    public class Project : ISoftDelete
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.INACTIVE;
        public Boolean IsDeleted { get; set; } = false;

        // Relationships

        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }

        public Guid? CurrentLeadId { get; set; }
        public virtual Member? CurrentLead { get; set; }

        public virtual ICollection<ProjectMember> TeamMembers { get; set; } = new List<ProjectMember>();

        // Constructors

        public Project() { }
        public Project(string name, string description, ProjectStatus status, Boolean isDeleted) {
            Name = name;
            Description = description;
            Status = status;
            IsDeleted = isDeleted;
        }
    }
}
