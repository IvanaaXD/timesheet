using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Activity;

namespace TimeSheet.Application.Abstractions
{
    public interface IActivityService
    {
        Task<ActivityDTO> GetActivityByIdAsync(Guid id);
        Task<ActivitySummaryDTO> GetActivitiesByDateAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<ActivityDTO>> GetAllActivitiesAsync();
        Task<ActivityDTO> CreateActivityAsync(ActivityRequestDTO activity);
        //Task UpdateActivityAsync(Activity activity);
        //Task DeleteActivityAsync(Activity activity);
        Task<IEnumerable<ActivityDTO>> SearchActivitiesAsync(
            Guid? memberId,
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate);
    }
}

