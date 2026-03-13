using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using TimeSheet.Domain.Entities.Enums;

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

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var result = await _memberService.GetMemberByIdAsync(id);
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

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberRequestDTO MemberRequestDTO)
        {
            var result = await _memberService.CreateMemberAsync(MemberRequestDTO);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember([FromBody] MemberRequestDTO MemberRequestDTO, Guid id)
        {
            var result = await _memberService.UpdateMemberAsync(id, MemberRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("password/{id}")]
        public async Task<IActionResult> UpdateMemberPassword(Guid id)
        {
            var result = await _memberService.UpdateMemberPasswordAsync(id);
            return Ok(result);
        }

        [HttpPut("forgot-password/{username}")]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            var result = await _memberService.ForgotPasswordAsync(username);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            await _memberService.DeleteMemberAsync(id);
            return NoContent();
        }
    }
}