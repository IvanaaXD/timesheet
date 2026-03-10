using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly TimeSheetDbContext _context;

        public ActivityRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        private IQueryable<Activity> GetActivitiesWithIncludes()
        {
            return _context.Activities
                .IgnoreQueryFilters()
                .Include(a => a.Member)
                .Include(a => a.Category)
                .Include(a => a.Project)
                    .ThenInclude(p => p.Client)
                .AsSplitQuery();
        }

        public async Task<Activity> FindActivityByIdAsync(Guid id)
        {
            return await GetActivitiesWithIncludes().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Activity>> FindActivitiesByDateAsync(DateOnly startDate, DateOnly endDate)
        {
            return await GetActivitiesWithIncludes()
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> FindAllActivitiesAsync()
        {
            return await GetActivitiesWithIncludes().AsNoTracking().ToListAsync();
        }

        public async Task<Activity> AddActivityAsync(Activity activity)
        {
            _context.Activities.Add(activity);  
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<IEnumerable<Activity>> SearchActivitiesAsync(
            Guid? memberId, 
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            IQueryable<Activity> query = _context.Activities;

            if (memberId.HasValue)
                query = query.Where(a => a.MemberId == memberId.Value);

            if (projectId.HasValue)
                query = query.Where(a => a.ProjectId == projectId.Value);

            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            if (clientId.HasValue)
                query = query.Where(a => a.Project.ClientId == clientId.Value);

            if (startDate.HasValue)
                query = query.Where(a => a.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Date <= endDate.Value);

            return await query
                .Include(a => a.Project)
                    .ThenInclude(p => p.Client)
                .Include(a => a.Member)
                .Include(a => a.Category)
                .OrderByDescending(a => a.Date).AsNoTracking()
                .ToListAsync();
        }
    }
}
