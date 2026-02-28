namespace InvoiceSystem.Application.DTOs.Client
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public required string Abn { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
