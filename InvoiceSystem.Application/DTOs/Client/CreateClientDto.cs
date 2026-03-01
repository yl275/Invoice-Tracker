namespace InvoiceSystem.Application.DTOs.Client
{
    public class CreateClientDto
    {
        public required string Abn { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Comment { get; set; }
    }
}
