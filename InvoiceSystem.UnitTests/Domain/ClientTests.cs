using FluentAssertions;
using Catamac.Domain.Entities;

namespace InvoiceSystem.UnitTests.Domain
{
    public class ClientTests
    {
        [Theory]
        [InlineData("", "Name", "0400")]
        [InlineData("ABN", "", "0400")]
        [InlineData("ABN", "Name", "")]
        public void Constructor_ShouldThrowException_WhenRequiredFieldsAreEmpty(string abn, string name, string phone)
        {
            // Act
            Action act = () => new Client(abn, name, phone);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_ShouldCreateClient_WhenDataIsValid()
        {
            // Arrange
            var abn = "123";
            var name = "Test";
            var phone = "0400";

            // Act
            var client = new Client(abn, name, phone);

            // Assert
            client.Should().NotBeNull();
            client.Abn.Should().Be(abn);
            client.Name.Should().Be(name);
            client.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void UpdateContactInfo_ShouldUpdateFields_WhenDataIsValid()
        {
            // Arrange
            var client = new Client("123", "Old Name", "Old Phone");

            // Act
            client.UpdateContactInfo("New Name", "New Phone");

            // Assert
            client.Name.Should().Be("New Name");
            client.PhoneNumber.Should().Be("New Phone");
        }
    }
}
