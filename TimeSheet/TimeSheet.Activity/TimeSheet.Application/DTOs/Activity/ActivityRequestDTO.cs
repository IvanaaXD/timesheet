using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Activity
{
    public class ActivityRequestDTO
    {
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public decimal Time { get; set; }
        public decimal OverTime { get; set; }

        // Relathionships

        public Guid ProjectId { get; set; }
        public Guid CategoryId { get; set; }

        // Constructors

        public ActivityRequestDTO() { }
        public ActivityRequestDTO(string description, DateOnly date, decimal time, decimal overtime)
        {
            Description = description;
            Date = date;
            Time = time;
            OverTime = overtime;
        }
    }
}
