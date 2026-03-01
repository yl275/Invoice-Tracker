using InvoiceSystem.Application.DTOs.BusinessProfile;
using InvoiceSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSystem.API.Controllers
{
    [ApiController]
    [Route("api/business-profile")]
    [Authorize]
    public class BusinessProfileController : ControllerBase
    {
        private readonly IBusinessProfileService _businessProfileService;

        public BusinessProfileController(IBusinessProfileService businessProfileService)
        {
            _businessProfileService = businessProfileService;
        }

        [HttpGet]
        public async Task<ActionResult<BusinessProfileDto>> GetMine()
        {
            var profile = await _businessProfileService.GetMyProfileAsync();
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<BusinessProfileDto>> Upsert(UpsertBusinessProfileDto dto)
        {
            try
            {
                var profile = await _businessProfileService.UpsertMyProfileAsync(dto);
                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
