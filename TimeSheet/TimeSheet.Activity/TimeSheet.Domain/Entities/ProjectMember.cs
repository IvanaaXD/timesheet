namespace TimeSheet.Domain.Entities
{
    public class ProjectMember
    {
        // Reationships

        public Guid MemberId { get; set; }
        public virtual Member Member { get; set; }

        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public bool IsLead { get; set; } = false;
    }
}
