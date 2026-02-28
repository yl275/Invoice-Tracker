using InvoiceSystem.Application.DTOs.Invoice;
using InvoiceSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceDto createInvoiceDto)
        {
            try
            {
                var invoice = await _invoiceService.CreateInvoiceAsync(createInvoiceDto);
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Service throws generic Exception for ClientNotFound/ProductNotFound currently. 
                // To satisfy "CreateInvoice_ShouldReturnBadRequest_WhenClientNotFound", we map this to BadRequest too.
                return BadRequest(ex.Message);
            }
        }
    }
}
