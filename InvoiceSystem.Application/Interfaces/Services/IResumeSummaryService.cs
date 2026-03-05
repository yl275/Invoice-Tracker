using InvoiceSystem.Application.DTOs.ResumeSummary;

namespace InvoiceSystem.Application.Interfaces.Services
{
    public interface IResumeSummaryService
    {
        TechSummaryResponseDto GenerateSummary(TechSummaryRequestDto request);
    }
}
