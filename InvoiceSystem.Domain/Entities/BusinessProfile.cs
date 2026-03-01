namespace InvoiceSystem.Domain.Entities;

public class BusinessProfile
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string PostalLocation { get; private set; }
    public string? Website { get; private set; }
    public string Abn { get; private set; }
    public string PaymentMethod { get; private set; } = null!;
    public string? BankBsb { get; private set; }
    public string? BankAccountNumber { get; private set; }
    public string? PayId { get; private set; }

    protected BusinessProfile()
    {
        Name = null!;
        Email = null!;
        Phone = null!;
        PostalLocation = null!;
        Abn = null!;
        PaymentMethod = null!;
    }

    public BusinessProfile(
        string userId,
        string name,
        string email,
        string phone,
        string postalLocation,
        string? website,
        string abn,
        string paymentMethod,
        string? bankBsb,
        string? bankAccountNumber,
        string? payId
    )
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone cannot be empty", nameof(phone));
        if (string.IsNullOrWhiteSpace(postalLocation)) throw new ArgumentException("Postal location cannot be empty", nameof(postalLocation));
        if (string.IsNullOrWhiteSpace(abn)) throw new ArgumentException("ABN cannot be empty", nameof(abn));

        Id = Guid.NewGuid();
        UserId = userId;
        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        PostalLocation = postalLocation.Trim();
        Website = NormalizeOptional(website);
        Abn = abn.Trim();

        SetPayment(paymentMethod, bankBsb, bankAccountNumber, payId);
    }

    public void Update(
        string name,
        string email,
        string phone,
        string postalLocation,
        string? website,
        string abn,
        string paymentMethod,
        string? bankBsb,
        string? bankAccountNumber,
        string? payId
    )
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone cannot be empty", nameof(phone));
        if (string.IsNullOrWhiteSpace(postalLocation)) throw new ArgumentException("Postal location cannot be empty", nameof(postalLocation));
        if (string.IsNullOrWhiteSpace(abn)) throw new ArgumentException("ABN cannot be empty", nameof(abn));

        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        PostalLocation = postalLocation.Trim();
        Website = NormalizeOptional(website);
        Abn = abn.Trim();

        SetPayment(paymentMethod, bankBsb, bankAccountNumber, payId);
    }

    private void SetPayment(string paymentMethod, string? bankBsb, string? bankAccountNumber, string? payId)
    {
        if (paymentMethod is not ("BankTransfer" or "PayId"))
            throw new ArgumentException("Payment method must be BankTransfer or PayId", nameof(paymentMethod));

        PaymentMethod = paymentMethod;

        if (paymentMethod == "BankTransfer")
        {
            if (string.IsNullOrWhiteSpace(bankBsb)) throw new ArgumentException("BSB is required for bank transfer", nameof(bankBsb));
            if (string.IsNullOrWhiteSpace(bankAccountNumber)) throw new ArgumentException("Account number is required for bank transfer", nameof(bankAccountNumber));

            BankBsb = bankBsb.Trim();
            BankAccountNumber = bankAccountNumber.Trim();
            PayId = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(payId))
            throw new ArgumentException("PayId is required when payment method is PayId", nameof(payId));

        PayId = payId.Trim();
        BankBsb = null;
        BankAccountNumber = null;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim();
    }
}
