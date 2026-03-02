using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using TimeSheet.Application.DTOs.ProjectLead;

namespace TimeSheet.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectLeadController : ControllerBase
    {
        private readonly IProjectLeadService _projectLeadService;

        public ProjectLeadController(IProjectLeadService projectLeadService)
        {
            _projectLeadService = projectLeadService;
        }

        [Authorize]
        [HttpPost("assign-lead")]
        public async Task<IActionResult> AssignLead([FromBody] ProjectLeadRequestDTO projectLeadDTO)
        {
            await _projectLeadService.AssignLeadAsync(projectLeadDTO.ProjectId, projectLeadDTO.MemberId);
            return Ok(new { message = "Lead successfully assigned." });
        }

        [Authorize]
        [HttpPost("remove-lead")]
        public async Task<IActionResult> RemoveLead([FromBody] ProjectLeadRequestDTO projectLeadDTO)
        {
            await _projectLeadService.RemoveLeadAsync(projectLeadDTO.ProjectId, projectLeadDTO.MemberId);
            return Ok(new { message = "Lead successfully removed." });
        }

        [Authorize]
        [HttpGet("project-by-leads")]
        public async Task<IActionResult> GetProjectsByLead(Guid memberId)
        {
            var result = await _projectLeadService.GetProjectsByLeadAsync(memberId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("leads-by-project")]
        public async Task<IActionResult> GetLeadsByProject(Guid projectId)
        {
            var result = await _projectLeadService.GetLeadsByProjectAsync(projectId);
            return Ok(result);
        }
    }
}