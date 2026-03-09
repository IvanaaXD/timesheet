using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using TimeSheet.Application.DTOs.ProjectMember;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectMemberController : ControllerBase
    {
        private readonly IProjectMemberService _projectMemberService;

        public ProjectMemberController(IProjectMemberService projectMemberService)
        {
            _projectMemberService = projectMemberService;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddMemberToProject([FromBody] ProjectMemberRequestDTO projectMemberRequestDTO)
        {
            await _projectMemberService.AddMemberToProjectAsync(projectMemberRequestDTO.ProjectId, projectMemberRequestDTO.MemberId, projectMemberRequestDTO.IsLead);
            return Ok(new { message = "Member successfully added to the project." });
        }

        [Authorize]
        [HttpPost("assign-lead")]
        public async Task<IActionResult> AssignLead([FromBody] ProjectMemberRequestDTO projectMemberRequestDTO)
        {
            await _projectMemberService.AssignLeadAsync(projectMemberRequestDTO.ProjectId, projectMemberRequestDTO.MemberId);
            return Ok(new { message = "Lead successfully assigned." });
        }


        [Authorize]
        [HttpPost("remove-lead")]
        public async Task<IActionResult> RemoveLead([FromBody] ProjectMemberRequestDTO projectMemberRequestDTO)
        {
            await _projectMemberService.RemoveLeadAsync(projectMemberRequestDTO.ProjectId, projectMemberRequestDTO.MemberId);
            return Ok(new { message = "Lead successfully removed from position." });
        }

        [Authorize]
        [HttpDelete("project/{projectId}/member/{memberId}")]
        public async Task<IActionResult> RemoveMemberFromProject([FromRoute] Guid projectId, [FromRoute] Guid memberId)
        {
            await _projectMemberService.RemoveMemberFromProjectAsync(projectId, memberId);
            return Ok(new { message = "Member successfully removed from the project." });
        }

        [Authorize]
        [HttpGet("member/{memberId}/projects")]
        public async Task<IActionResult> GetProjectsByMember([FromRoute] Guid memberId)
        {
            var result = await _projectMemberService.GetProjectsByMemberAsync(memberId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("project/{projectId}/members")]
        public async Task<IActionResult> GetMembersByProject([FromRoute] Guid projectId)
        {
            var result = await _projectMemberService.GetMembersByProjectAsync(projectId);
            return Ok(result);
        }      
    }
}