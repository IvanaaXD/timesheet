using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> FindProjectByIdAsync(Guid id);
        Task<IEnumerable<Project>> FindAllProjectsAsync();
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(Project project);
        Task<PagedList<Project>> FindAllProjectsPagedAsync(
                int pageNumber,
                int pageSize,
                string? searchTerm,
                string? firstLetter,
                string order);
    }
}
