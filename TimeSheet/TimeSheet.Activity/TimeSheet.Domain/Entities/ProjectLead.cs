namespace TimeSheet.Domain.Entities
{
    public class ProjectLead
    {
        // Reationships

        public Guid MemberId { get; set; }
        public virtual Member Member { get; set; }

        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}
