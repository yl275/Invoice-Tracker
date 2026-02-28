using FluentAssertions;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.UnitTests;

namespace InvoiceSystem.UnitTests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_ShouldThrowException_WhenPriceIsZeroOrNegative()
        {
            Action act = () => new Product(TestData.UserId, "Name", "SKU", 0m);
            act.Should().Throw<ArgumentException>().WithMessage("*Price must be greater than zero*");
        }

        [Fact]
        public void UpdatePrice_ShouldThrowException_WhenPriceIsNegative()
        {
            var product = new Product(TestData.UserId, "Name", "SKU", 10m);
            Action act = () => product.UpdatePrice(-5m);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateNameAndSku()
        {
            var product = new Product(TestData.UserId, "Old Name", "Old SKU", 10m);
            product.UpdateDetails("New Name", "New SKU");
            product.Name.Should().Be("New Name");
            product.SKU.Should().Be("New SKU");
        }
    }
}
