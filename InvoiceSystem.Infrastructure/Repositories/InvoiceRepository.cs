using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .Include(i => i.Items) // Eager load items
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Invoice>> ListAsync()
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .ToListAsync();
        }
    }
}
