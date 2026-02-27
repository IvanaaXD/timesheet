using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Project;
using TimeSheet.Application.Common.DTOs;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Project with ID {id} not found." });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _projectService.GetAllProjectsAsync();
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedProjects([FromQuery] PagedListDTO pagedListDTO)
        {
            var result = await _projectService.GetAllProjectsPagedAsync(pagedListDTO);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDTO ProjectRequestDTO)
        {
            var result = await _projectService.CreateProjectAsync(ProjectRequestDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectRequestDTO ProjectRequestDTO, Guid id)
        {
            var result = await _projectService.UpdateProjectAsync(id, ProjectRequestDTO);
            return Ok(result);
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}