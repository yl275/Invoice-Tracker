using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Interfaces.Repositories
{
    public interface IBusinessProfileRepository
    {
        Task<BusinessProfile?> GetByUserIdAsync(string userId);
        Task AddAsync(BusinessProfile profile);
        Task UpdateAsync(BusinessProfile profile);
    }
}
