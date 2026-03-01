namespace InvoiceSystem.Application.DTOs.BusinessProfile
{
    public class BusinessProfileDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string PostalLocation { get; set; }
        public string? Website { get; set; }
        public required string Abn { get; set; }
        public required string PaymentMethod { get; set; }
        public string? BankBsb { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? PayId { get; set; }
    }
}
