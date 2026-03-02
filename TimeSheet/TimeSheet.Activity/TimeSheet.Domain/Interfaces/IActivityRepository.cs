using System;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IActivityRepository
    {
        Task<Activity> FindActivityByIdAsync(Guid id);
        Task<IEnumerable<Activity>> FindActivitiesByDateAsync(DateOnly date);
        Task<IEnumerable<Activity>> FindAllActivitiesAsync();
        Task AddActivityAsync(Activity activity);
        //Task UpdateActivityAsync(Activity activity);
        //Task DeleteActivityAsync(Activity activity);
        Task<IEnumerable<Activity>> SearchActivitiesAsync(
            Guid? memberId,
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate);
    }
}
