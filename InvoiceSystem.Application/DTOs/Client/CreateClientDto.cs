namespace InvoiceSystem.Application.DTOs.Client
{
    public class CreateClientDto
    {
        public required string Abn { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
