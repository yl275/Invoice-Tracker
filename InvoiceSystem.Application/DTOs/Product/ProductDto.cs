namespace InvoiceSystem.Application.DTOs.Product

{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string SKU { get; set; }
        public decimal Price { get; set; }
    }
}
