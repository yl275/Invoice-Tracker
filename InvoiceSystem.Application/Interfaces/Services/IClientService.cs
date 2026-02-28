using InvoiceSystem.Application.DTOs.Client;

namespace InvoiceSystem.Application.Interfaces.Services
{
    public interface IClientService
    {
        Task<ClientDto> GetByIdAsync(Guid id);
        Task<ClientDto> RegisterClientAsync(CreateClientDto createClientDto);
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task UpdateClientAsync(Guid id, UpdateClientDto updateClientDto);
    }
}
