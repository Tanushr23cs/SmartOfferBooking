using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Business;
using SmartOfferBooking.Application.Interfaces.Services;

namespace SmartOfferBooking.API.Controllers;

[ApiController]
[Route("api/business")]
public class BusinessController : ControllerBase
{
    private readonly IBusinessProfileService _businessProfileService;

    public BusinessController(IBusinessProfileService businessProfileService)
    {
        _businessProfileService = businessProfileService;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<BusinessProfileDto>>> Create(
        [FromBody] CreateBusinessProfileDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _businessProfileService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(Get), ApiResponse<BusinessProfileDto>.Ok(result, "Business profile created"));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BusinessProfileDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await _businessProfileService.GetAsync(cancellationToken);
        if (result is null)
            return NotFound(ApiResponse<BusinessProfileDto>.Fail("Business profile not found"));

        return Ok(ApiResponse<BusinessProfileDto>.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BusinessProfileDto>>> Update(
        Guid id,
        [FromBody] UpdateBusinessProfileDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _businessProfileService.UpdateAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<BusinessProfileDto>.Ok(result, "Business profile updated"));
    }
}
