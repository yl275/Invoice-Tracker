namespace InvoiceSystem.Application.DTOs.Product

{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
    }
}
