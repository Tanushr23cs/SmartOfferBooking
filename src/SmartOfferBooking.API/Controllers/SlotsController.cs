using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Slots;
using SmartOfferBooking.Application.Interfaces.Services;

namespace SmartOfferBooking.API.Controllers;

[ApiController]
[Route("api")]
public class SlotsController : ControllerBase
{
    private readonly ISlotService _slotService;

    public SlotsController(ISlotService slotService)
    {
        _slotService = slotService;
    }

    [HttpPost("slots")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<SlotDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<SlotDto>>> Create(
        [FromBody] CreateSlotDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _slotService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByOfferId), new { offerId = result.OfferId }, ApiResponse<SlotDto>.Ok(result, "Slot created"));
    }

    [HttpGet("slots")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SlotDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SlotDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _slotService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SlotDto>>.Ok(result));
    }

    [HttpGet("offers/{offerId:guid}/slots")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SlotDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SlotDto>>>> GetByOfferId(
        Guid offerId,
        CancellationToken cancellationToken)
    {
        var result = await _slotService.GetByOfferIdAsync(offerId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SlotDto>>.Ok(result));
    }

    [HttpPut("slots/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<SlotDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SlotDto>>> Update(
        Guid id,
        [FromBody] UpdateSlotDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _slotService.UpdateAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<SlotDto>.Ok(result, "Slot updated"));
    }

    [HttpDelete("slots/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _slotService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Slot cancelled"));
    }
}
