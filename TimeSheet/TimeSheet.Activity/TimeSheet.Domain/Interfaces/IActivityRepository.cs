using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Domain.Interfaces
{
    public interface IActivityRepository
    {
        Task<Activity> FindActivityByIdAsync(Guid id);
        Task<IEnumerable<Activity>> FindActivitiesByDateAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Activity>> FindAllActivitiesAsync();
        Task<Activity> AddActivityAsync(Activity activity);
        //Task UpdateActivityAsync(Activity activity);
        //Task DeleteActivityAsync(Activity activity);
        Task<PagedList<Activity>> SearchActivitiesAsync(
            Guid? memberId,
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate,
            int pageNumber,
            int pageSize,
            string order = "desc");       
    }
}
