using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Country;

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

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetCountryById(Guid id)
        {
            var result = await _countryService.GetCountryByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Country with ID {id} not found." });
            }

            return Ok(result);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCountryByName(string name)
        {
            var result = await _countryService.GetCountryByNameAsync(name);

            if (result == null)
            {
                return NotFound(new { message = $"Country with ID {name} not found." });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var result = await _countryService.GetAllCountriesAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CountryRequestDTO CountryRequestDTO)
        {
            var result = await _countryService.CreateCountryAsync(CountryRequestDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryRequestDTO CountryRequestDTO, Guid id)
        {
            var result = await _countryService.UpdateCountryAsync(id, CountryRequestDTO);
            return Ok(result);
        }
    }
}