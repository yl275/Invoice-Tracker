namespace InvoiceSystem.Domain.Entities;

public class InvoiceItem
{
    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string SKU { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public decimal Total { get; private set; }

    protected InvoiceItem()
    {
        ProductName = null!;
        SKU = null!;
    }

    public InvoiceItem(Guid invoiceId, Product product, int quantity)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Id = Guid.NewGuid();
        InvoiceId = invoiceId;
        ProductId = product.Id;
        ProductName = product.Name;
        SKU = product.SKU;
        Price = product.Price; // Snapshot of the price at the time of adding to invoice
        Quantity = quantity;

        Total = Price * Quantity;
    }
}
