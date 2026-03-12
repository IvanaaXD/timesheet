using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Activity
{
    public class ReportQueryDTO
    {
        public Guid? MemberId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? CategoryId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Constructors

        public ReportQueryDTO() { }
        public ReportQueryDTO(Guid? memberId, Guid? clientId, Guid? projectId, Guid? categoryId, DateOnly? startDate, DateOnly? endDate)
        {
            MemberId = memberId;
            ClientId = clientId;
            ProjectId = projectId;
            CategoryId = categoryId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
