namespace InvoiceSystem.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string SKU { get; private set; }
    public decimal Price { get; private set; }

    protected Product()
    {
        Name = null!;
        SKU = null!;
    }

    public Product(string name, string sku, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU cannot be empty", nameof(sku));
        if (price <= 0) throw new ArgumentException("Price must be greater than zero", nameof(price));

        Id = Guid.NewGuid();
        Name = name;
        SKU = sku;
        Price = price;
    }

    public void UpdateDetails(string name, string sku)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU cannot be empty", nameof(sku));

        Name = name;
        SKU = sku;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative", nameof(newPrice));
        Price = newPrice;
    }
}
