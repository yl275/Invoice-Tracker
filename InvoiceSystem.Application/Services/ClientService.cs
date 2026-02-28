using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Application.DTOs.Client;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Interfaces.Services;

namespace InvoiceSystem.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ClientDto?> GetByIdAsync(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null) return null;

            return new ClientDto
            {
                Id = client.Id,
                Abn = client.Abn,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber
            };
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _clientRepository.ListAsync();
            return clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Abn = c.Abn,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber
            });
        }

        public async Task<ClientDto> RegisterClientAsync(CreateClientDto createClientDto)
        {
            var client = new Client(createClientDto.Abn, createClientDto.Name, createClientDto.PhoneNumber);
            await _clientRepository.AddAsync(client);

            return new ClientDto
            {
                Id = client.Id,
                Abn = client.Abn,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber
            };
        }

        public async Task UpdateClientAsync(Guid id, UpdateClientDto updateClientDto)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client != null)
            {
                client.UpdateContactInfo(updateClientDto.Name, updateClientDto.PhoneNumber);
                await _clientRepository.UpdateAsync(client);
            }
        }
    }
}
