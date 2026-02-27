using System;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> FindProjectByIdAsync(Guid id);
        Task<IEnumerable<Project>> FindAllProjectsAsync();
        Task AddProjectAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(Project project);
        Task<PagedList<Project>> FindAllProjectsPagedAsync(
                int pageNumber,
                int pageSize,
                string? searchTerm,
                string? firstLetter,
                string order);
    }
}
