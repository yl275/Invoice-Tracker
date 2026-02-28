using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InvoiceSystem.Application.DTOs.Client;
using Xunit;

namespace InvoiceSystem.IntegrationTests
{
    public class ClientsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClientsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateClient_ShouldReturnCreated_WhenDataIsValid()
        {
            // Arrange
            var createDto = new CreateClientDto { Abn = "123456789", Name = "Test Client", PhoneNumber = "0400111222" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/clients", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var client = await response.Content.ReadFromJsonAsync<ClientDto>();
            client.Should().NotBeNull();
            client!.Name.Should().Be("Test Client");
        }

        [Fact]
        public async Task CreateClient_ShouldReturnBadRequest_WhenRequiredFieldMissing()
        {
            // Arrange
            var createDto = new CreateClientDto { Abn = "", Name = "", PhoneNumber = "" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/clients", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetClientById_ShouldReturnClient_WhenExists()
        {
            // Arrange
            var createDto = new CreateClientDto { Abn = "ExistABN", Name = "Existing Client", PhoneNumber = "0400000000" };
            var createResponse = await _client.PostAsJsonAsync("/api/clients", createDto);
            var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

            // Act
            var response = await _client.GetAsync($"/api/clients/{createdClient!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ClientDto>();
            result.Should().BeEquivalentTo(createdClient);
        }

        [Fact]
        public async Task GetClientById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"/api/clients/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllClients_ShouldReturnList()
        {
            // Arrange
            await _client.PostAsJsonAsync("/api/clients", new CreateClientDto { Abn = "List1", Name = "List Client 1", PhoneNumber = "111" });
            await _client.PostAsJsonAsync("/api/clients", new CreateClientDto { Abn = "List2", Name = "List Client 2", PhoneNumber = "222" });

            // Act
            var response = await _client.GetAsync("/api/clients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
            list.Should().NotBeNullOrEmpty();
            list.Should().Contain(c => c.Name == "List Client 1");
            list.Should().Contain(c => c.Name == "List Client 2");
        }
    }
}
