namespace InvoiceSystem.Application.DTOs.Invoice
{
    public class InvoiceItemDto
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
