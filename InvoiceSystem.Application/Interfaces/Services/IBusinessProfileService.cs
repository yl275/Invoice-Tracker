using InvoiceSystem.Application.DTOs.BusinessProfile;

namespace InvoiceSystem.Application.Interfaces.Services
{
    public interface IBusinessProfileService
    {
        Task<BusinessProfileDto?> GetMyProfileAsync();
        Task<BusinessProfileDto> UpsertMyProfileAsync(UpsertBusinessProfileDto dto);
    }
}
