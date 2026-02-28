using Catamac.Domain.Entities;

namespace InvoiceSystem.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task<IEnumerable<Product>> ListAsync();
        Task DeleteAsync(Guid id);
        Task UpdateAsync(Product product);

    }
}
