using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Activity;
using Microsoft.AspNetCore.Authorization;
using TimeSheet.Domain.Entities.Enums;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetActivityById(Guid id)
        {
            var result = await _activityService.GetActivityByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetActivityByDate(DateOnly date)
        {
            var result = await _activityService.GetActivitiesByDateAsync(date);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var result = await _activityService.GetAllActivitiesAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityRequestDTO ActivityRequestDTO)
        {
            var result = await _activityService.CreateActivityAsync(ActivityRequestDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("search")]
        public async Task<IActionResult> GetAllActivitiesS([FromBody] ReportQueryDTO reportQueryDTO)
        {
            var result = await _activityService.SearchActivitiesAsync(
                reportQueryDTO.MemberId,
                reportQueryDTO.ClientId,
                reportQueryDTO.ProjectId,
                reportQueryDTO.CategoryId,
                reportQueryDTO.StartDate,
                reportQueryDTO.EndDate);
            return Ok(result);
        }
    }
}