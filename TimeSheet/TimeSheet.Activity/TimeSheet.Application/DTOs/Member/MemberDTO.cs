using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities.Enums;

namespace TimeSheet.Application.DTOs.Member
{
    public class MemberDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public decimal HoursPerWeek { get; set; }
        public MemberStatus Status { get; set; }
        public MemberRole Role { get; set; }

        // Constructors

        public MemberDTO() { }
        public MemberDTO(string name, string username, string email, string password, decimal hoursPerWeek, MemberStatus status, MemberRole role)
        {
            Name = name;
            Username = username;
            Email = email;
            HoursPerWeek = hoursPerWeek;
            Status = status;
            Role = role;
        }
    }
}
