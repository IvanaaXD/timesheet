using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Client;
using TimeSheet.Application.Common.DTOs;

namespace TimeSheet.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var result = await _clientService.GetClientByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Client with ID {id} not found." });
            }

            return Ok(result);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetClientByName(string name)
        {
            var result = await _clientService.GetClientByNameAsync(name);

            if (result == null)
            {
                return NotFound(new { message = $"Client with name {name} not found." });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var result = await _clientService.GetAllClientsAsync();
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedClients([FromQuery] PagedListDTO pagedListDTO)
        {
            var result = await _clientService.GetAllClientsPagedAsync(pagedListDTO);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientRequestDTO clientRequestDTO)
        {
            var result = await _clientService.CreateClientAsync(clientRequestDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient([FromBody] ClientRequestDTO clientRequestDTO, Guid id)
        {
            var result = await _clientService.UpdateClientAsync(id, clientRequestDTO);
            return Ok(result);
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteClient(Guid id) 
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}