using InvoiceSystem.Application.DTOs.ResumeSummary;
using InvoiceSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeSummaryController : ControllerBase
    {
        private readonly IResumeSummaryService _resumeSummaryService;

        public ResumeSummaryController(IResumeSummaryService resumeSummaryService)
        {
            _resumeSummaryService = resumeSummaryService;
        }

        /// <summary>
        /// Generates concise resume bullet points from a list of detected technologies.
        /// </summary>
        [HttpPost]
        public ActionResult<TechSummaryResponseDto> GenerateSummary(TechSummaryRequestDto request)
        {
            if (request.Technologies == null || request.Technologies.Count == 0)
                return BadRequest("At least one technology must be provided.");

            var result = _resumeSummaryService.GenerateSummary(request);
            return Ok(result);
        }
    }
}
