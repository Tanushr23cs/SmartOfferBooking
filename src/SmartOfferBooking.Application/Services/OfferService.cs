using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Offers;
using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Application.Helpers;
using SmartOfferBooking.Application.Interfaces;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Application.Mappings;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IBusinessProfileRepository _businessRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingNotificationService _notificationService;

    public OfferService(
        IOfferRepository offerRepository,
        IBusinessProfileRepository businessRepository,
        ISlotRepository slotRepository,
        IUnitOfWork unitOfWork,
        IBookingNotificationService notificationService)
    {
        _offerRepository = offerRepository;
        _businessRepository = businessRepository;
        _slotRepository = slotRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<OfferDto> CreateAsync(CreateOfferDto dto, CancellationToken cancellationToken = default)
    {
        OfferValidator.ValidatePricing(dto.OriginalPrice, dto.OfferPrice);
        if (dto.EndDate < dto.StartDate)
            throw new ValidationException("EndDate must be on or after StartDate.");

        var entity = MapCreate(dto);
        entity.Status = OfferValidator.ResolveStatus(entity);

        await _offerRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await NotifyOfferAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<PagedResult<OfferDto>> GetPagedAsync(OfferQueryDto query, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _offerRepository.GetPagedAsync(query, cancellationToken);
        foreach (var o in items)
            o.Status = OfferValidator.ResolveStatus(o);

        return new PagedResult<OfferDto>
        {
            Items = items.Select(o => o.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    public async Task<PagedResult<OfferPublicDto>> GetPublicPagedAsync(OfferQueryDto query, CancellationToken cancellationToken = default)
    {
        query.PublicOnly = true;
        var (items, total) = await _offerRepository.GetPagedAsync(query, cancellationToken);
        var business = await _businessRepository.GetFirstAsync(cancellationToken);
        var seatMap = await _offerRepository.GetAvailableSeatsByOfferIdsAsync(items.Select(o => o.Id), cancellationToken);
        var slotCounts = await GetAvailableSlotCountsAsync(items.Select(o => o.Id), cancellationToken);

        var dtos = items.Select(o =>
        {
            o.Status = OfferValidator.ResolveStatus(o);
            seatMap.TryGetValue(o.Id, out var seats);
            slotCounts.TryGetValue(o.Id, out var slots);
            return MapPublic(o, business, seats, slots);
        }).ToList();

        return new PagedResult<OfferPublicDto>
        {
            Items = dtos,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    public async Task<OfferDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _offerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");
        entity.Status = OfferValidator.ResolveStatus(entity);
        return entity.ToDto();
    }

    public async Task<OfferPublicDto> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _offerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");

        if (entity.Status == OfferStatus.Cancelled)
            throw new NotFoundException("Offer not found.");

        entity.Status = OfferValidator.ResolveStatus(entity);
        if (entity.Status != OfferStatus.Active)
            throw new NotFoundException("Offer is not available.");

        var business = await _businessRepository.GetFirstAsync(cancellationToken);
        var seatMap = await _offerRepository.GetAvailableSeatsByOfferIdsAsync(new[] { id }, cancellationToken);
        var slotCounts = await GetAvailableSlotCountsAsync(new[] { id }, cancellationToken);
        seatMap.TryGetValue(id, out var seats);
        slotCounts.TryGetValue(id, out var slots);
        return MapPublic(entity, business, seats, slots);
    }

    public async Task<OfferDto> UpdateAsync(Guid id, UpdateOfferDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _offerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");

        OfferValidator.ValidatePricing(dto.OriginalPrice, dto.OfferPrice);
        if (dto.EndDate < dto.StartDate)
            throw new ValidationException("EndDate must be on or after StartDate.");

        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description.Trim();
        entity.Category = dto.Category.Trim();
        entity.OriginalPrice = dto.OriginalPrice;
        entity.OfferPrice = dto.OfferPrice;
        entity.DiscountPercentage = OfferValidator.CalculateDiscountPercentage(dto.OriginalPrice, dto.OfferPrice);
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.TotalCapacity = dto.TotalCapacity;
        entity.MaxBookingPerCustomer = dto.MaxBookingPerCustomer;
        entity.TermsAndConditions = dto.TermsAndConditions;
        entity.Status = dto.Status;
        entity.Status = OfferValidator.ResolveStatus(entity);
        entity.UpdatedAt = DateTime.UtcNow;

        await _offerRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await NotifyOfferAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _offerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");

        entity.Status = OfferStatus.Cancelled;
        entity.UpdatedAt = DateTime.UtcNow;
        await _offerRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await NotifyOfferAsync(entity, cancellationToken);
    }

    private async Task NotifyOfferAsync(Offer offer, CancellationToken cancellationToken)
    {
        var seatMap = await _offerRepository.GetAvailableSeatsByOfferIdsAsync(new[] { offer.Id }, cancellationToken);
        seatMap.TryGetValue(offer.Id, out var seats);
        await _notificationService.NotifyOfferUpdatedAsync(offer, seats, cancellationToken);
    }

    private async Task<Dictionary<Guid, int>> GetAvailableSlotCountsAsync(
        IEnumerable<Guid> offerIds,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, int>();
        foreach (var offerId in offerIds)
        {
            var slots = await _slotRepository.GetByOfferIdAsync(offerId, cancellationToken);
            result[offerId] = slots.Count(s => s.Status == SlotStatus.Available && s.AvailableCount > 0);
        }
        return result;
    }

    private static Offer MapCreate(CreateOfferDto dto) => new()
    {
        Title = dto.Title.Trim(),
        Description = dto.Description.Trim(),
        Category = dto.Category.Trim(),
        OriginalPrice = dto.OriginalPrice,
        OfferPrice = dto.OfferPrice,
        DiscountPercentage = OfferValidator.CalculateDiscountPercentage(dto.OriginalPrice, dto.OfferPrice),
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        TotalCapacity = dto.TotalCapacity,
        MaxBookingPerCustomer = dto.MaxBookingPerCustomer,
        TermsAndConditions = dto.TermsAndConditions,
        Status = dto.Status
    };

    private static OfferPublicDto MapPublic(Offer o, BusinessProfile? business, int seats, int slots) => new()
    {
        Id = o.Id,
        Title = o.Title,
        Description = o.Description,
        Category = o.Category,
        OriginalPrice = o.OriginalPrice,
        OfferPrice = o.OfferPrice,
        DiscountPercentage = o.DiscountPercentage,
        StartDate = o.StartDate,
        EndDate = o.EndDate,
        StartTime = o.StartTime,
        EndTime = o.EndTime,
        TotalCapacity = o.TotalCapacity,
        MaxBookingPerCustomer = o.MaxBookingPerCustomer,
        TermsAndConditions = o.TermsAndConditions,
        Status = o.Status,
        BusinessName = business?.BusinessName ?? "Smart Offer Partner",
        BusinessType = business?.BusinessType ?? "General",
        LogoUrl = business?.LogoUrl,
        City = business?.City ?? "",
        Address = business?.Address,
        AvailableSlotsCount = slots,
        TotalAvailableSeats = seats,
        CreatedAt = o.CreatedAt
    };
}
