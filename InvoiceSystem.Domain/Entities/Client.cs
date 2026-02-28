namespace InvoiceSystem.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string Abn { get; private set; }
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }

    // Navigation property
    private readonly List<Invoice> _invoices = new();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    // Constructor for EF Core
    protected Client()
    {
        Abn = null!;
        Name = null!;
        PhoneNumber = null!;
    }

    public Client(string userId, string abn, string name, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(abn)) throw new ArgumentException("ABN cannot be empty", nameof(abn));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("Phone Number cannot be empty", nameof(phoneNumber));

        Id = Guid.NewGuid();
        UserId = userId;
        Abn = abn;
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public void UpdateContactInfo(string name, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("Phone Number cannot be empty", nameof(phoneNumber));

        Name = name;
        PhoneNumber = phoneNumber;
    }
}
