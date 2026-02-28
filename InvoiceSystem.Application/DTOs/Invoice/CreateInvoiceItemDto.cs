namespace InvoiceSystem.Application.DTOs.Invoice
{
    public class CreateInvoiceItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
