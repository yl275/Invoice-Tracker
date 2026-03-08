using FluentAssertions;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.UnitTests;

namespace InvoiceSystem.UnitTests.Domain
{
    public class InvoiceTests
    {
        [Fact]
        public void Constructor_ShouldSnapshotClientDetails()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN123", "Client Name", "0400");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-001", DateTime.Now, client, profile);
            invoice.ClientAbnSnapshot.Should().Be(client.Abn);
            invoice.ClientNameSnapshot.Should().Be(client.Name);
            invoice.ClientPhoneSnapshot.Should().Be(client.PhoneNumber);
        }

        [Fact]
        public void AddItem_ShouldIncreaseTotalAmount()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN", "Name", "Phone");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-001", DateTime.Now, client, profile);
            var product = new Product(TestData.TestTeamId, TestData.UserId, "Widget", "SKU", 10m);
            invoice.AddItem(product, 2);
            invoice.Items.Should().HaveCount(1);
            invoice.TotalAmount.Should().Be(20m);
        }

        [Fact]
        public void TotalAmount_ShouldSumMultipleItems()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN", "Name", "Phone");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-001", DateTime.Now, client, profile);
            var p1 = new Product(TestData.TestTeamId, TestData.UserId, "P1", "S1", 10m);
            var p2 = new Product(TestData.TestTeamId, TestData.UserId, "P2", "S2", 50m);
            invoice.AddItem(p1, 3);
            invoice.AddItem(p2, 1);
            invoice.TotalAmount.Should().Be(80m);
        }

        [Fact]
        public void AddItem_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN", "Name", "Phone");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-001", DateTime.Now, client, profile);
            var product = new Product(TestData.TestTeamId, TestData.UserId, "Widget", "SKU", 10m);
            Action act = () => invoice.AddItem(product, 0);
            act.Should().Throw<ArgumentException>().WithMessage("*Quantity must be greater than zero*");
        }

        [Fact]
        public void AddItem_ShouldAllowDuplicateProducts_AsSeparateLines()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN", "Name", "Phone");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-001", DateTime.Now, client, profile);
            var product = new Product(TestData.TestTeamId, TestData.UserId, "Widget", "SKU", 10m);
            invoice.AddItem(product, 1);
            invoice.AddItem(product, 2);
            invoice.Items.Should().HaveCount(2);
            invoice.TotalAmount.Should().Be(30m);
        }

        [Fact]
        public void RemoveItem_ShouldDecreaseTotalAmount()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN", "Name", "Phone");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-001", DateTime.Now, client, profile);
            var product = new Product(TestData.TestTeamId, TestData.UserId, "Widget", "SKU", 10m);
            invoice.AddItem(product, 2);
            invoice.RemoveItem(product.Id);
            invoice.Items.Should().BeEmpty();
            invoice.TotalAmount.Should().Be(0m);
        }

        [Fact]
        public void TotalAmount_ShouldHandleHighPrecision()
        {
            var client = new Client(TestData.TestTeamId, TestData.UserId, "ABN", "Name", "Phone");
            var profile = new BusinessProfile(
                TestData.UserId,
                "Biz",
                "biz@test.local",
                "0400",
                "Addr",
                null,
                "ABN123",
                "BankTransfer",
                "123-456",
                "123456789",
                null);
            var invoice = new Invoice(TestData.TestTeamId, TestData.UserId, "INV-Precise", DateTime.Now, client, profile);
            var p1 = new Product(TestData.TestTeamId, TestData.UserId, "Precise Widget", "SKU-P", 10.1234m);
            invoice.AddItem(p1, 2);
            invoice.TotalAmount.Should().Be(20.2468m);
        }
    }
}
