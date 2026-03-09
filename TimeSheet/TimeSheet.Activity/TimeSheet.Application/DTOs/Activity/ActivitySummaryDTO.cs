using System.Collections.Generic;

namespace TimeSheet.Application.DTOs.Activity
{
    public class ActivitySummaryDTO
    {
        public IEnumerable<ActivityDTO> Activities { get; set; } = new List<ActivityDTO>();
        public decimal TotalHours { get; set; }
    }
}