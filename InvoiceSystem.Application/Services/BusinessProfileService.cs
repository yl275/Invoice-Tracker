using InvoiceSystem.Application.DTOs.BusinessProfile;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Interfaces.Services;
using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Services
{
    public class BusinessProfileService : IBusinessProfileService
    {
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly IUserContext _userContext;

        public BusinessProfileService(IBusinessProfileRepository businessProfileRepository, IUserContext userContext)
        {
            _businessProfileRepository = businessProfileRepository;
            _userContext = userContext;
        }

        public async Task<BusinessProfileDto?> GetMyProfileAsync()
        {
            if (!_userContext.HasUser) return null;

            var profile = await _businessProfileRepository.GetByUserIdAsync(_userContext.UserId!);
            if (profile == null) return null;

            return ToDto(profile);
        }

        public async Task<BusinessProfileDto> UpsertMyProfileAsync(UpsertBusinessProfileDto dto)
        {
            if (!_userContext.HasUser)
                throw new UnauthorizedAccessException("User must be authenticated to update business profile.");

            var userId = _userContext.UserId!;
            var existing = await _businessProfileRepository.GetByUserIdAsync(userId);

            if (existing == null)
            {
                var profile = new BusinessProfile(
                    userId,
                    dto.Name,
                    dto.Email,
                    dto.Phone,
                    dto.PostalLocation,
                    dto.Website,
                    dto.Abn,
                    dto.PaymentMethod,
                    dto.BankBsb,
                    dto.BankAccountNumber,
                    dto.PayId
                );

                await _businessProfileRepository.AddAsync(profile);
                return ToDto(profile);
            }

            existing.Update(
                dto.Name,
                dto.Email,
                dto.Phone,
                dto.PostalLocation,
                dto.Website,
                dto.Abn,
                dto.PaymentMethod,
                dto.BankBsb,
                dto.BankAccountNumber,
                dto.PayId
            );

            await _businessProfileRepository.UpdateAsync(existing);
            return ToDto(existing);
        }

        private static BusinessProfileDto ToDto(BusinessProfile profile)
        {
            return new BusinessProfileDto
            {
                Id = profile.Id,
                Name = profile.Name,
                Email = profile.Email,
                Phone = profile.Phone,
                PostalLocation = profile.PostalLocation,
                Website = profile.Website,
                Abn = profile.Abn,
                PaymentMethod = profile.PaymentMethod,
                BankBsb = profile.BankBsb,
                BankAccountNumber = profile.BankAccountNumber,
                PayId = profile.PayId
            };
        }
    }
}
