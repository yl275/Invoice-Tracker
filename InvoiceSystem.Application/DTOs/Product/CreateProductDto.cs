namespace InvoiceSystem.Application.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
    }
}
