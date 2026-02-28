using FluentAssertions;
using Catamac.Domain.Entities;

namespace InvoiceSystem.UnitTests.Domain
{
    public class InvoiceTests
    {
        [Fact]
        public void Constructor_ShouldSnapshotClientDetails()
        {
            // Arrange
            var client = new Client("ABN123", "Client Name", "0400");

            // Act
            var invoice = new Invoice("INV-001", DateTime.Now, client);

            // Assert
            invoice.ClientAbnSnapshot.Should().Be(client.Abn);
            invoice.ClientNameSnapshot.Should().Be(client.Name);
            invoice.ClientPhoneSnapshot.Should().Be(client.PhoneNumber);
        }

        [Fact]
        public void AddItem_ShouldIncreaseTotalAmount()
        {
            // Arrange
            var client = new Client("ABN", "Name", "Phone");
            var invoice = new Invoice("INV-001", DateTime.Now, client);
            var product = new Product("Widget", "SKU", 10m);

            // Act
            invoice.AddItem(product, 2); // 2 * 10 = 20

            // Assert
            invoice.Items.Should().HaveCount(1);
            invoice.TotalAmount.Should().Be(20m);
        }

        [Fact]
        public void TotalAmount_ShouldSumMultipleItems()
        {
            // Arrange
            var client = new Client("ABN", "Name", "Phone");
            var invoice = new Invoice("INV-001", DateTime.Now, client);
            var p1 = new Product("P1", "S1", 10m);
            var p2 = new Product("P2", "S2", 50m);

            // Act
            invoice.AddItem(p1, 3); // 30
            invoice.AddItem(p2, 1); // 50

            // Assert
            invoice.TotalAmount.Should().Be(80m);
        }

        [Fact]
        public void AddItem_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            // Arrange
            var client = new Client("ABN", "Name", "Phone");
            var invoice = new Invoice("INV-001", DateTime.Now, client);
            var product = new Product("Widget", "SKU", 10m);

            // Act
            Action act = () => invoice.AddItem(product, 0);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Quantity must be greater than zero*");
        }

        [Fact]
        public void AddItem_ShouldAllowDuplicateProducts_AsSeparateLines()
        {
            // Note: Depending on business rule. Current implementation allows separate lines.
            // Arrange
            var client = new Client("ABN", "Name", "Phone");
            var invoice = new Invoice("INV-001", DateTime.Now, client);
            var product = new Product("Widget", "SKU", 10m);

            // Act
            invoice.AddItem(product, 1);
            invoice.AddItem(product, 2);

            // Assert
            invoice.Items.Should().HaveCount(2);
            invoice.TotalAmount.Should().Be(30m); // 10 + 20
        }

        [Fact]
        public void RemoveItem_ShouldDecreaseTotalAmount()
        {
            // Arrange
            var client = new Client("ABN", "Name", "Phone");
            var invoice = new Invoice("INV-001", DateTime.Now, client);
            var product = new Product("Widget", "SKU", 10m);
            invoice.AddItem(product, 2); // Total 20

            // Act
            invoice.RemoveItem(product.Id);

            // Assert
            invoice.Items.Should().BeEmpty();
            invoice.TotalAmount.Should().Be(0m);
        }

        [Fact]
        public void TotalAmount_ShouldHandleHighPrecision()
        {
            // Arrange
            var client = new Client("ABN", "Name", "Phone");
            var invoice = new Invoice("INV-Precise", DateTime.Now, client);
            var p1 = new Product("Precise Widget", "SKU-P", 10.1234m);

            // Act
            invoice.AddItem(p1, 2);

            // Assert
            invoice.TotalAmount.Should().Be(20.2468m);
        }
    }
}
