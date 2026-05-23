using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Offers;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.API.Controllers;

[ApiController]
[Route("api/offers")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<OfferDto>>> Create(
        [FromBody] CreateOfferDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _offerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OfferDto>.Ok(result, "Offer created"));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] string? businessType = null,
        [FromQuery] OfferStatus? status = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool availableOnly = false,
        [FromQuery] bool publicOnly = false,
        [FromQuery] DateOnly? slotDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new OfferQueryDto
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Category = category,
            BusinessType = businessType,
            Status = status,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            AvailableOnly = availableOnly,
            PublicOnly = publicOnly || User.Identity?.IsAuthenticated != true,
            SlotDate = slotDate
        };

        if (query.PublicOnly)
        {
            var publicResult = await _offerService.GetPublicPagedAsync(query, cancellationToken);
            return Ok(ApiResponse<PagedResult<OfferPublicDto>>.Ok(publicResult));
        }

        var adminResult = await _offerService.GetPagedAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<OfferDto>>.Ok(adminResult));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetById(
        Guid id,
        [FromQuery] bool publicView = false,
        CancellationToken cancellationToken = default)
    {
        if (publicView || User.Identity?.IsAuthenticated != true)
        {
            var publicOffer = await _offerService.GetPublicByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<OfferPublicDto>.Ok(publicOffer));
        }

        var offer = await _offerService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<OfferDto>.Ok(offer));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<OfferDto>>> Update(
        Guid id,
        [FromBody] UpdateOfferDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _offerService.UpdateAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<OfferDto>.Ok(result, "Offer updated"));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _offerService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Offer cancelled"));
    }
}
