namespace InvoiceSystem.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        public required string Name { get; set; }
        public required string SKU { get; set; }
        public decimal Price { get; set; }
    }
}
