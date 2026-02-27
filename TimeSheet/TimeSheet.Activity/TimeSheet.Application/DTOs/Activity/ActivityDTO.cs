using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Activity
{
	public class ActivityDTO
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public decimal Time { get; set; }
        public decimal OverTime { get; set; }

        // Relathionships

        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string CategoryName { get; set; }
        public string MemberName { get; set; }

        // Constructors

        public ActivityDTO() { }
        public ActivityDTO(string description, DateOnly date, decimal time, decimal overtime)
        {
            Description = description;
            Date = date;
            Time = time;
            OverTime = overtime;
        }
    }
}
