using FluentAssertions;
using Catamac.Domain.Entities;

namespace InvoiceSystem.UnitTests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_ShouldThrowException_WhenPriceIsZeroOrNegative()
        {
            // Act
            Action act = () => new Product("Name", "SKU", 0m);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Price must be greater than zero*");
        }

        [Fact]
        public void UpdatePrice_ShouldThrowException_WhenPriceIsNegative()
        {
            // Arrange
            var product = new Product("Name", "SKU", 10m);

            // Act
            Action act = () => product.UpdatePrice(-5m);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateNameAndSku()
        {
            // Arrange
            var product = new Product("Old Name", "Old SKU", 10m);

            // Act
            product.UpdateDetails("New Name", "New SKU");

            // Assert
            product.Name.Should().Be("New Name");
            product.SKU.Should().Be("New SKU");
        }
    }
}
