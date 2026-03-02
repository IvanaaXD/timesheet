using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace TimeSheet.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService MemberService)
        {
            _memberService = MemberService;
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var result = await _memberService.GetMemberByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Member with ID {id} not found." });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetMemberByUsername(string username)
        {
            var result = await _memberService.GetMemberByUsernameAsync(username);

            if (result == null)
            {
                return NotFound(new { message = $"Member with username {username} not found." });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetMemberByEmail(string email)
        {
            var result = await _memberService.GetMemberByEmailAsync(email);

            if (result == null)
            {
                return NotFound(new { message = $"Member with email {email} not found." });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var result = await _memberService.GetAllMembersAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("paged")]
        public async Task<IActionResult> GetAllMembersPaged([FromQuery] PagedListDTO pagedListDTO)
        {
            var result = await _memberService.GetAllMembersPagedAsync(pagedListDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberRequestDTO MemberRequestDTO)
        {
            var result = await _memberService.CreateMemberAsync(MemberRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember([FromBody] MemberRequestDTO MemberRequestDTO, Guid id)
        {
            var result = await _memberService.UpdateMemberAsync(id, MemberRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("password/{id}")]
        public async Task<IActionResult> UpdateMemberPassword([FromBody] MemberRequestDTO MemberRequestDTO, Guid id)
        {
            var result = await _memberService.UpdateMemberPasswordAsync(id, MemberRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            await _memberService.DeleteMemberAsync(id);
            return NoContent();
        }
    }
}