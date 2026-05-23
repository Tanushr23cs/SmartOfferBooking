using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Bookings;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<BookingDto>>> Create(
        [FromBody] CreateBookingDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<BookingDto>.Ok(result, "Booking created"));
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<PagedResult<BookingDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] Guid? offerId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.GetPagedAsync(new BookingQueryDto
        {
            Page = page,
            PageSize = pageSize,
            Status = status,
            OfferId = offerId
        }, cancellationToken);

        return Ok(ApiResponse<PagedResult<BookingDto>>.Ok(result));
    }

    [HttpGet("recent")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BookingDetailDto>>>> GetRecent(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.GetRecentDetailsAsync(limit, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BookingDetailDto>>.Ok(result));
    }

    [HttpGet("reference/{reference}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> GetByReference(
        string reference,
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetByReferenceAsync(reference, cancellationToken);
        return Ok(ApiResponse<BookingDetailDto>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateBookingStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.UpdateStatusAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking status updated"));
    }
}
