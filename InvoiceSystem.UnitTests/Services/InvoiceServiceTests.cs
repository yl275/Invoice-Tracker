using FluentAssertions;
using InvoiceSystem.Application.DTOs.Invoice;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Services;
using Moq;
using Catamac.Domain.Entities;

namespace InvoiceSystem.UnitTests.Services
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly InvoiceService _invoiceService;

        public InvoiceServiceTests()
        {
            _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _invoiceService = new InvoiceService(
                _invoiceRepositoryMock.Object,
                _clientRepositoryMock.Object,
                _productRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldThrowException_WhenClientNotFound()
        {
            // Arrange
            var createDto = new CreateInvoiceDto { ClientId = Guid.NewGuid(), InvoiceCode = "TEST-INV" };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ClientId))
                .ReturnsAsync((Client)null!);

            // Act
            Func<Task> act = async () => await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Client not found");
        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldCreateInvoice_WhenDataIsValid()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var client = new Client("ABN", "Test", "PH");
            var product = new Product("Widget", "SKU", 10m); // Price 10

            // We need to set IDs to match (impossible with private set in tests without reflection or helper, but for flow it's ok)
            // Ideally we mock the return to ensure the service logic uses the object we give it.

            var createDto = new CreateInvoiceDto
            {
                ClientId = clientId,
                InvoiceCode = "INV-001",
                InvoiceDate = DateTime.UtcNow.AddDays(7),
                Items = new List<CreateInvoiceItemDto>
                {
                    new CreateInvoiceItemDto { ProductId = productId, Quantity = 2 } // Total should be 20
                }
            };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().Total.Should().Be(20m); // 2 * 10

        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldThrowException_WhenProductNotFound()
        {
            // ... method content ...
            // Arrange
            var clientId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var client = new Client("ABN", "Test", "PH");

            var createDto = new CreateInvoiceDto
            {
                ClientId = clientId,
                InvoiceCode = "INV-Test",
                InvoiceDate = DateTime.Now,
                Items = new List<CreateInvoiceItemDto>
                {
                    new CreateInvoiceItemDto { ProductId = productId, Quantity = 1 }
                }
            };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product)null!);

            // Act
            Func<Task> act = async () => await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage($"Product with ID {productId} not found.");
        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var client = new Client("ABN", "Test", "PH");

            var createDto = new CreateInvoiceDto
            {
                ClientId = clientId,
                InvoiceCode = "INV-Test",
                InvoiceDate = DateTime.Now,
                Items = new List<CreateInvoiceItemDto>
                {
                    new CreateInvoiceItemDto { ProductId = productId, Quantity = 0 }
                }
            };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);

            // Act
            Func<Task> act = async () => await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage($"Quantity for product {productId} must be greater than zero.");
        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldThrowException_WhenItemsAreEmpty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client("ABN", "Test", "PH");

            var createDto = new CreateInvoiceDto
            {
                ClientId = clientId,
                InvoiceCode = "INV-Test",
                InvoiceDate = DateTime.Now,
                Items = new List<CreateInvoiceItemDto>() // Empty
            };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);

            // Act
            Func<Task> act = async () => await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Invoice must contain at least one item.");
        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldCalculateTotalCorrectly_WhenMultipleItemsAdded()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var p1Id = Guid.NewGuid();
            var p2Id = Guid.NewGuid();

            var client = new Client("ABN", "Test", "PH");
            var p1 = new Product("P1", "SKU1", 10m);
            var p2 = new Product("P2", "SKU2", 50m);

            var createDto = new CreateInvoiceDto
            {
                ClientId = clientId,
                InvoiceCode = "INV-Total",
                InvoiceDate = DateTime.Now,
                Items = new List<CreateInvoiceItemDto>
                {
                    new CreateInvoiceItemDto { ProductId = p1Id, Quantity = 3 }, // 3 * 10 = 30
                    new CreateInvoiceItemDto { ProductId = p2Id, Quantity = 1 }  // 1 * 50 = 50
                }
            };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(p1Id)).ReturnsAsync(p1);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(p2Id)).ReturnsAsync(p2);

            // Act
            var result = await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            result.TotalAmount.Should().Be(80m);
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateInvoiceAsync_ShouldBubbleUpRepositoryException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var createDto = new CreateInvoiceDto { ClientId = clientId, InvoiceCode = "TEST-INV" };

            _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ThrowsAsync(new InvalidOperationException("DB Error"));

            // Act
            Func<Task> act = async () => await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("DB Error");
        }


        [Fact]
        public async Task GetInvoiceAsync_ShouldReturnInvoiceDto_WhenInvoiceExists()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var client = new Client("ABN", "Client", "PH");
            var invoice = new Invoice("INV-001", DateTime.Now, client);
            // Add an item manually to test mapping if public/internal method available, or constructor
            var product = new Product("P1", "S1", 10m);
            invoice.AddItem(product, 2);

            _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.GetInvoiceAsync(invoiceId);

            // Assert
            result.Should().NotBeNull();
            result.InvoiceCode.Should().Be("INV-001");
            result.Items.Should().HaveCount(1);
            result.Items.First().ProductName.Should().Be("P1");
        }

        [Fact]
        public async Task GetInvoiceAsync_ShouldReturnNull_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId)).ReturnsAsync((Invoice)null!);

            // Act
            var result = await _invoiceService.GetInvoiceAsync(invoiceId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllInvoicesAsync_ShouldReturnAllInvoices()
        {
            // Arrange
            var client = new Client("ABN", "Client", "PH");
            var invoices = new List<Invoice>
            {
                new Invoice("INV-1", DateTime.Now, client),
                new Invoice("INV-2", DateTime.Now, client)
            };

            _invoiceRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(invoices);

            // Act
            var result = await _invoiceService.GetAllInvoicesAsync();

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
