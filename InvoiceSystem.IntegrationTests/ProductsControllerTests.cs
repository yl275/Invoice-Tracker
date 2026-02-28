using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InvoiceSystem.Application.DTOs.Product;
using Xunit;

namespace InvoiceSystem.IntegrationTests
{
    public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreated_WhenDataIsValid()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Valid Product", SKU = "VP-001", Price = 100m };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            product.Should().NotBeNull();
            product!.Name.Should().Be("Valid Product");
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenPriceNegativeOrNameEmpty()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "", SKU = "", Price = -10m };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Existing Product", SKU = "EP-001", Price = 50m };
            var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Act
            var response = await _client.GetAsync($"/api/products/{createdProduct!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ProductDto>();
            result.Should().BeEquivalentTo(createdProduct);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnList()
        {
            // Arrange
            await _client.PostAsJsonAsync("/api/products", new CreateProductDto { Name = "List P1", SKU = "LP-1", Price = 10m });
            await _client.PostAsJsonAsync("/api/products", new CreateProductDto { Name = "List P2", SKU = "LP-2", Price = 20m });

            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            list.Should().NotBeNullOrEmpty();
            list.Should().Contain(p => p.Name == "List P1");
            list.Should().Contain(p => p.Name == "List P2");
        }
    }
}
