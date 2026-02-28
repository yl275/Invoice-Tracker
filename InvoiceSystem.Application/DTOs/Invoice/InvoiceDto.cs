namespace InvoiceSystem.Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
