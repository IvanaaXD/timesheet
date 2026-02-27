using System.Xml.Linq;

namespace TimeSheet.Domain.Entities
{
    public class Activity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateOnly Date {  get; set; } 
        public decimal Time { get; set; }
        public decimal OverTime { get; set; }

        // Relathionships

        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public Guid MemberId { get; set; }
        public virtual Member Member { get; set; }

        // Constructors

        public Activity() { }
        public Activity(string description, DateOnly date, decimal time, decimal overtime)
        {
            Description = description;
            Date = date;
            Time = time;
            OverTime = overtime;
        }
    }
}
