using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Project;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Application.Common.DTOs;

namespace TimeSheet.Application.Abstractions
{
    public interface IProjectService
    {
        Task<ProjectDTO> GetProjectByIdAsync(Guid id);
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
        Task<ProjectDTO> CreateProjectAsync(ProjectRequestDTO request);
        Task<ProjectDTO> UpdateProjectAsync(Guid id, ProjectRequestDTO request);
        Task DeleteProjectAsync(Guid id);
        Task<PagedList<ProjectDTO>> GetAllProjectsPagedAsync(PagedListDTO pagedListDTO);
    }
}
