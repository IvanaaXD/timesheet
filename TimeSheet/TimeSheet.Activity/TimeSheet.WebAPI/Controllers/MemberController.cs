using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.Common.DTOs;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService MemberService)
        {
            _memberService = MemberService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var result = await _memberService.GetAllMembersAsync();
            return Ok(result);
        }

        //[HttpGet("paged")]
        //public async Task<IActionResult> GetAllMembersPaged()
        //{
        //    var result = await _memberService.GetAllMembersPagedAsync();
        //    return Ok(result);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberRequestDTO MemberRequestDTO)
        {
            var result = await _memberService.CreateMemberAsync(MemberRequestDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember([FromBody] MemberRequestDTO MemberRequestDTO, Guid id)
        {
            var result = await _memberService.UpdateMemberAsync(id, MemberRequestDTO);
            return Ok(result);
        }

        [HttpPut("password/{id}")]
        public async Task<IActionResult> UpdateMemberPassword([FromBody] MemberRequestDTO MemberRequestDTO, Guid id)
        {
            var result = await _memberService.UpdateMemberPasswordAsync(id, MemberRequestDTO);
            return Ok(result);
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            try
            {
                await _memberService.DeleteMemberAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}