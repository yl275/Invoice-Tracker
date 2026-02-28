using InvoiceSystem.Application.DTOs.Invoice;

namespace InvoiceSystem.Application.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto);
        Task<InvoiceDto> GetInvoiceAsync(Guid id);
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    }
}
