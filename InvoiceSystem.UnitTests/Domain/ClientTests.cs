using FluentAssertions;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.UnitTests;

namespace InvoiceSystem.UnitTests.Domain
{
    public class ClientTests
    {
        [Theory]
        [InlineData(TestData.UserId, "", "Name", "0400")]
        [InlineData(TestData.UserId, "ABN", "", "0400")]
        [InlineData(TestData.UserId, "ABN", "Name", "")]
        public void Constructor_ShouldThrowException_WhenRequiredFieldsAreEmpty(string userId, string abn, string name, string phone)
        {
            Action act = () => new Client(userId, abn, name, phone);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_ShouldCreateClient_WhenDataIsValid()
        {
            var abn = "123";
            var name = "Test";
            var phone = "0400";

            var client = new Client(TestData.UserId, abn, name, phone);

            client.Should().NotBeNull();
            client.Abn.Should().Be(abn);
            client.Name.Should().Be(name);
            client.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void UpdateContactInfo_ShouldUpdateFields_WhenDataIsValid()
        {
            var client = new Client(TestData.UserId, "123", "Old Name", "Old Phone");

            client.UpdateContactInfo("New Name", "New Phone");

            client.Name.Should().Be("New Name");
            client.PhoneNumber.Should().Be("New Phone");
        }
    }
}
