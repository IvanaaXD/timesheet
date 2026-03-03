using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Client;
using TimeSheet.Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using TimeSheet.Domain.Entities.Enums;

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

        [Authorize]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetClientByName(string name)
        {
            var result = await _clientService.GetClientByNameAsync(name);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var result = await _clientService.GetAllClientsAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedClients([FromQuery] PagedListDTO pagedListDTO)
        {
            var result = await _clientService.GetAllClientsPagedAsync(pagedListDTO);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientRequestDTO clientRequestDTO)
        {
            var result = await _clientService.CreateClientAsync(clientRequestDTO);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient([FromBody] ClientRequestDTO clientRequestDTO, Guid id)
        {
            var result = await _clientService.UpdateClientAsync(id, clientRequestDTO);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id) 
        {
            await _clientService.DeleteClientAsync(id);
            return NoContent();
        }
    }
}