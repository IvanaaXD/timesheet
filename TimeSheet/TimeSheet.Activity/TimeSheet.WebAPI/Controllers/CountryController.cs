using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Country;
using Microsoft.AspNetCore.Authorization;
using TimeSheet.Domain.Entities.Enums;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetCountryById(Guid id)
        {
            var result = await _countryService.GetCountryByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCountryByName(string name)
        {
            var result = await _countryService.GetCountryByNameAsync(name);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var result = await _countryService.GetAllCountriesAsync();
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CountryRequestDTO CountryRequestDTO)
        {
            var result = await _countryService.CreateCountryAsync(CountryRequestDTO);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryRequestDTO CountryRequestDTO, Guid id)
        {
            var result = await _countryService.UpdateCountryAsync(id, CountryRequestDTO);
            return Ok(result);
        }
    }
}