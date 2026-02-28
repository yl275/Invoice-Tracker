using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Interfaces.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(Guid id);
        Task AddAsync(Invoice invoice);
        Task<IEnumerable<Invoice>> ListAsync();
    }
}
