using FluentAssertions;
using InvoiceSystem.Application.DTOs.Client;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Services;
using Moq;
using Catamac.Domain.Entities;

namespace InvoiceSystem.UnitTests.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly ClientService _clientService;

        public ClientServiceTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _clientService = new ClientService(_clientRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterClientAsync_ShouldReturnClientDto_WhenClientIsRegistered()
        {
            // Arrange
            var createDto = new CreateClientDto
            {
                Abn = "123456789",
                Name = "Test Client",
                PhoneNumber = "0400000000"
            };

            // Act
            var result = await _clientService.RegisterClientAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Abn.Should().Be(createDto.Abn);
            result.Name.Should().Be(createDto.Name);
            result.PhoneNumber.Should().Be(createDto.PhoneNumber);
            result.Id.Should().NotBeEmpty();

            _clientRepositoryMock.Verify(x => x.AddAsync(It.Is<Client>(c =>
                c.Abn == createDto.Abn &&
                c.Name == createDto.Name)), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnClientDto_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client("123", "Test Client", "0400");
            // Set the ID via reflection since it's private set, or rely on Entity behavior
            // Since we can't easily set protected/private set properties without reflection or constructor
            // Assuming Client constructor or EF sets ID. Entity Base usually has protected set.
            // For this test, we might just match properties.
            // *Wait, Client entity has Id generated in constructor usually or base class.*
            // Let's assume Entity has Id.

            // Mocking the repo
            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId))
                .ReturnsAsync(client);

            // Act
            var result = await _clientService.GetByIdAsync(clientId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Client");
            _clientRepositoryMock.Verify(x => x.GetByIdAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId))
                .ReturnsAsync((Client)null!);

            // Act
            var result = await _clientService.GetByIdAsync(clientId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllClientsAsync_ShouldReturnAllClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client("123", "Client 1", "04001"),
                new Client("456", "Client 2", "04002")
            };

            _clientRepositoryMock.Setup(x => x.ListAsync())
                .ReturnsAsync(clients);

            // Act
            var result = await _clientService.GetAllClientsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Client 1");
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldUpdateClient_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client("ABN", "Old Name", "Old Phone");
            var updateDto = new UpdateClientDto { Name = "New Name", PhoneNumber = "New Phone" };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId))
                .ReturnsAsync(client);

            // Act
            await _clientService.UpdateClientAsync(clientId, updateDto);

            // Assert
            client.Name.Should().Be(updateDto.Name);
            client.PhoneNumber.Should().Be(updateDto.PhoneNumber);
            _clientRepositoryMock.Verify(x => x.UpdateAsync(client), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldDoNothing_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var updateDto = new UpdateClientDto { Name = "New", PhoneNumber = "New" };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId))
                .ReturnsAsync((Client)null!);

            // Act
            await _clientService.UpdateClientAsync(clientId, updateDto);

            // Assert
            _clientRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Client>()), Times.Never);
        }
        [Fact]
        public async Task RegisterClientAsync_ShouldThrowException_WhenAbnIsEmpty()
        {
            // Arrange
            var createDto = new CreateClientDto { Abn = "", Name = "Test", PhoneNumber = "123" };

            // Act
            Func<Task> act = async () => await _clientService.RegisterClientAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("ABN cannot be empty*");
        }

        [Fact]
        public async Task RegisterClientAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var createDto = new CreateClientDto { Abn = "123", Name = "", PhoneNumber = "123" };

            // Act
            Func<Task> act = async () => await _clientService.RegisterClientAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Name cannot be empty*");
        }

        [Fact]
        public async Task RegisterClientAsync_ShouldThrowException_WhenPhoneNumberIsEmpty()
        {
            // Arrange
            var createDto = new CreateClientDto { Abn = "123", Name = "Test", PhoneNumber = "" };

            // Act
            Func<Task> act = async () => await _clientService.RegisterClientAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Phone Number cannot be empty*");
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client("123", "Old", "123");
            var updateDto = new UpdateClientDto { Name = "", PhoneNumber = "New" };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);

            // Act
            Func<Task> act = async () => await _clientService.UpdateClientAsync(clientId, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Name cannot be empty*");
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldThrowException_WhenPhoneNumberIsEmpty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client("123", "Old", "123");
            var updateDto = new UpdateClientDto { Name = "New", PhoneNumber = "" };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);

            // Act
            Func<Task> act = async () => await _clientService.UpdateClientAsync(clientId, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Phone Number cannot be empty*");
        }
    }
}
