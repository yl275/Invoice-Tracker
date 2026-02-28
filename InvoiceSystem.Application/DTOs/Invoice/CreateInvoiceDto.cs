namespace InvoiceSystem.Application.DTOs.Invoice
{
    public class CreateInvoiceDto
    {
        public required string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Guid ClientId { get; set; }
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }
}
