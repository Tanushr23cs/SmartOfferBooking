using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Waitlist;
using SmartOfferBooking.Application.Interfaces.Services;

namespace SmartOfferBooking.API.Controllers;

[ApiController]
[Route("api/waitlist")]
public class WaitlistController : ControllerBase
{
    private readonly IWaitlistService _waitlistService;

    public WaitlistController(IWaitlistService waitlistService)
    {
        _waitlistService = waitlistService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<WaitlistDto>>> Create(
        [FromBody] CreateWaitlistDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _waitlistService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<WaitlistDto>.Ok(result, "Added to waitlist"));
    }

    [HttpGet("offer/{offerId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WaitlistDto>>>> GetByOffer(
        Guid offerId,
        CancellationToken cancellationToken)
    {
        var result = await _waitlistService.GetByOfferAsync(offerId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<WaitlistDto>>.Ok(result));
    }
}
