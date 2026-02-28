namespace InvoiceSystem.Application.DTOs.Client
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Abn { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
