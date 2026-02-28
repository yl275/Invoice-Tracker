using InvoiceSystem.Application.DTOs.Client;
using InvoiceSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetById(Guid id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return Ok(client);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> Register(CreateClientDto createClientDto)
        {
            try
            {
                var client = await _clientService.RegisterClientAsync(createClientDto);
                return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClientDto updateClientDto)
        {
            await _clientService.UpdateClientAsync(id, updateClientDto);
            return NoContent();
        }
    }
}
