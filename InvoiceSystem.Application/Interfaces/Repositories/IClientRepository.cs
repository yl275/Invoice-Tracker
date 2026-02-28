using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(Guid id);
        Task AddAsync(Client client);
        Task<IEnumerable<Client>> ListAsync();
        Task DeleteAsync(Guid id);
        Task UpdateAsync(Client client);

    }
}
