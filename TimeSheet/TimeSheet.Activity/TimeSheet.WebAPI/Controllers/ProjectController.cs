using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Project;
using TimeSheet.Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace TimeSheet.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _projectService.GetAllProjectsAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedProjects([FromQuery] PagedListDTO pagedListDTO)
        {
            var result = await _projectService.GetAllProjectsPagedAsync(pagedListDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDTO ProjectRequestDTO)
        {
            var result = await _projectService.CreateProjectAsync(ProjectRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectRequestDTO ProjectRequestDTO, Guid id)
        {
            var result = await _projectService.UpdateProjectAsync(id, ProjectRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}