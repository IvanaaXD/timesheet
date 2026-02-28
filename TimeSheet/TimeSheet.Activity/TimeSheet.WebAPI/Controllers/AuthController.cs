using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Auth;
using TimeSheet.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request);

            if (result.StatusCode == 401)
            {
                return Unauthorized(new { message = "Incorrect credentials." });
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(new { message = "Username does not exist." });
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            await _identityService.LogoutAsync(request);
            return NoContent();
        }
    }
}