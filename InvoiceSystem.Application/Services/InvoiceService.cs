using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Application.DTOs.Invoice;
using InvoiceSystem.Application.DTOs.Client;
using InvoiceSystem.Application.DTOs.Product;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Interfaces.Services;

namespace InvoiceSystem.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto)
        {
            var client = await _clientRepository.GetByIdAsync(createInvoiceDto.ClientId);
            if (client == null) throw new Exception("Client not found");

            var invoice = new Invoice(createInvoiceDto.InvoiceCode, createInvoiceDto.InvoiceDate, client);

            if (createInvoiceDto.Items == null || !createInvoiceDto.Items.Any())
                throw new ArgumentException("Invoice must contain at least one item.");

            foreach (var itemDto in createInvoiceDto.Items)
            {
                if (itemDto.Quantity <= 0)
                    throw new ArgumentException($"Quantity for product {itemDto.ProductId} must be greater than zero.");

                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {itemDto.ProductId} not found.");

                invoice.AddItem(product, itemDto.Quantity);
            }

            await _invoiceRepository.AddAsync(invoice);

            // Return updated DTO 
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceDate = invoice.InvoiceDate,
                ClientId = invoice.ClientId,
                ClientName = invoice.ClientNameSnapshot,
                ClientAbn = invoice.ClientAbnSnapshot,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    SKU = i.SKU,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Total = i.Total
                }).ToList()
            };
        }

        public async Task<InvoiceDto?> GetInvoiceAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return null;

            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceDate = invoice.InvoiceDate,
                ClientId = invoice.ClientId,
                ClientName = invoice.ClientNameSnapshot,
                ClientAbn = invoice.ClientAbnSnapshot,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    SKU = i.SKU,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Total = i.Total
                }).ToList()
            };
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoices = await _invoiceRepository.ListAsync();

            // Note: ListAsync might not include Items/Client depending on Repo implementation
            // For now, mapping basic properties
            return invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceCode = i.InvoiceCode,
                InvoiceDate = i.InvoiceDate,
                ClientId = i.ClientId,
                ClientName = i.ClientNameSnapshot,
                ClientAbn = i.ClientAbnSnapshot,
                TotalAmount = i.TotalAmount
            });
        }
    }
}
