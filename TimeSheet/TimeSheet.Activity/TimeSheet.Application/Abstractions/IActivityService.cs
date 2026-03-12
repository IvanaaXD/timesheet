using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Activity;
using TimeSheet.Domain.Common.Models;

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
        Task<PagedList<ActivityDTO>> SearchActivitiesAsync(
            Guid? memberId,
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate,
            int pageNumber,
            int pageSize);
    }
}

