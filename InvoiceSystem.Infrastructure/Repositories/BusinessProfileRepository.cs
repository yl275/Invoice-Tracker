using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Infrastructure.Repositories
{
    public class BusinessProfileRepository : IBusinessProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public BusinessProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.BusinessProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task AddAsync(BusinessProfile profile)
        {
            await _context.BusinessProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BusinessProfile profile)
        {
            _context.BusinessProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}
