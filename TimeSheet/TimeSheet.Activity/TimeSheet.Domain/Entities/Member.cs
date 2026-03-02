using TimeSheet.Domain.Entities.Enums;
using TimeSheet.Domain.Interfaces;

namespace TimeSheet.Domain.Entities
{
    public class Member : ISoftDelete
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public decimal HoursPerWeek { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.INACTIVE;
        public MemberRole Role { get; set; }
        public Boolean IsDeleted { get; set; } = false;

        // Relationships

        public virtual ICollection<ProjectLead> LeadingAssignments { get; set; }

        // Constructors

        public Member() { }
        public Member(string name, string username, string email, string password, decimal hoursPerWeek, MemberStatus status, MemberRole role, Boolean isDeleted)
        {
            Name = name;
            Username = username;
            Email = email;
            Password = password;
            HoursPerWeek = hoursPerWeek;
            Status = status;
            Role = role;
            IsDeleted = isDeleted;
        }
    }
}
