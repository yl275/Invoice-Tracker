using FluentAssertions;
using InvoiceSystem.Application.DTOs.Product;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Services;
using Moq;
using Catamac.Domain.Entities;

namespace InvoiceSystem.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task AddProductAsync_ShouldReturnProductDto_WhenProductIsAdded()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Widget",
                SKU = "WID-123",
                Price = 10.00m
            };

            // Act
            var result = await _productService.AddProductAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createDto.Name);
            result.SKU.Should().Be(createDto.SKU);
            result.Price.Should().Be(createDto.Price);

            _productRepositoryMock.Verify(x => x.AddAsync(It.Is<Product>(p =>
                p.Name == createDto.Name && p.Price == createDto.Price)), Times.Once);
        }
        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenPriceIsZeroOrNegative()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Bad Product",
                SKU = "BAD-001",
                Price = -5m
            };

            // Act
            Func<Task> act = async () => await _productService.AddProductAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Price must be greater than zero.");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProductDto_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product("Widget", "SKU", 10m);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Widget");
            result.Price.Should().Be(10m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product)null!);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product("P1", "S1", 10m),
                new Product("P2", "S2", 20m)
            };
            _productRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product("Old Name", "Old SKU", 5m);
            var updateDto = new UpdateProductDto
            {
                Name = "New Name",
                SKU = "New SKU",
                Price = 15m
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            product.Name.Should().Be("New Name");
            product.SKU.Should().Be("New SKU");
            product.Price.Should().Be(15m);
            _productRepositoryMock.Verify(x => x.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto { Name = "N", SKU = "S", Price = 1 };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product)null!);

            // Act
            await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }
        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenPriceIsNegative()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product("Name", "SKU", 10m);
            var updateDto = new UpdateProductDto { Name = "New Name", SKU = "New SKU", Price = -5m };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Price cannot be negative*");
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product("Name", "SKU", 10m);
            var updateDto = new UpdateProductDto { Name = "", SKU = "New SKU", Price = 10m };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Name cannot be empty*");
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenSKUIsEmpty()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product("Name", "SKU", 10m);
            var updateDto = new UpdateProductDto { Name = "New Name", SKU = "", Price = 10m };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("SKU cannot be empty*");
        }
    }
}
