namespace InvoiceSystem.Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public required string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAbn { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAbn { get; set; } = string.Empty;
        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessPhone { get; set; } = string.Empty;
        public string BusinessPostalLocation { get; set; } = string.Empty;
        public string? BusinessWebsite { get; set; }
        public string BusinessPaymentMethod { get; set; } = string.Empty;
        public string? BusinessBankBsb { get; set; }
        public string? BusinessBankAccountNumber { get; set; }
        public string? BusinessPayId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
