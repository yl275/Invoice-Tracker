namespace InvoiceSystem.Domain.Entities;

public class Invoice
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string InvoiceCode { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }

    public Guid ClientId { get; private set; }
    public Client Client { get; private set; }

    // As client details may change
    public string ClientAbnSnapshot { get; private set; }
    public string ClientNameSnapshot { get; private set; }
    public string ClientPhoneSnapshot { get; private set; }

    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(i => i.Total); // Can exapand with taxes, discounts, etc.

    protected Invoice()
    {
        InvoiceCode = null!;
        Client = null!;
        ClientAbnSnapshot = null!;
        ClientNameSnapshot = null!;
        ClientPhoneSnapshot = null!;
    }

    public Invoice(string userId, string invoiceCode, DateTime invoiceDate, Client client, DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(invoiceCode)) throw new ArgumentException("Invoice Code cannot be empty", nameof(invoiceCode));
        if (client == null) throw new ArgumentNullException(nameof(client));
        var resolvedDueDate = dueDate ?? invoiceDate.Date.AddDays(30);
        if (resolvedDueDate < invoiceDate.Date) throw new ArgumentException("Due date cannot be earlier than invoice date", nameof(dueDate));

        Id = Guid.NewGuid();
        UserId = userId;
        InvoiceCode = invoiceCode;
        InvoiceDate = invoiceDate;
        DueDate = resolvedDueDate;

        Client = client;
        ClientId = client.Id;
        ClientAbnSnapshot = client.Abn;
        ClientNameSnapshot = client.Name;
        ClientPhoneSnapshot = client.PhoneNumber;
    }

    public void AddItem(Product product, int quantity)
    {
        // Check if item already exists with same product?    
        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id && i.Price == product.Price);

        var item = new InvoiceItem(this.Id, product, quantity);
        _items.Add(item);
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}
