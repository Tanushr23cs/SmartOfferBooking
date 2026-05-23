using SmartOfferBooking.Application.DTOs.Slots;
using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Application.Interfaces;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Application.Mappings;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Services;

public class SlotService : ISlotService
{
    private readonly ISlotRepository _slotRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingNotificationService _notificationService;

    public SlotService(
        ISlotRepository slotRepository,
        IOfferRepository offerRepository,
        IUnitOfWork unitOfWork,
        IBookingNotificationService notificationService)
    {
        _slotRepository = slotRepository;
        _offerRepository = offerRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<SlotDto> CreateAsync(CreateSlotDto dto, CancellationToken cancellationToken = default)
    {
        var offer = await _offerRepository.GetByIdAsync(dto.OfferId, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");

        if (offer.Status == OfferStatus.Cancelled)
            throw new ValidationException("Cannot add slots to a cancelled offer.");

        if (dto.EndTime <= dto.StartTime)
            throw new ValidationException("Slot EndTime must be after StartTime.");

        var entity = new Slot
        {
            OfferId = dto.OfferId,
            SlotDate = dto.SlotDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Capacity = dto.Capacity,
            BookedCount = 0,
            AvailableCount = dto.Capacity,
            Status = SlotStatus.Available
        };

        await _slotRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifySlotUpdatedAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<IReadOnlyList<SlotDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var slots = await _slotRepository.GetAllAsync(cancellationToken);
        return slots.Select(s => s.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<SlotDto>> GetByOfferIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");

        if (offer.Status == OfferStatus.Cancelled)
            throw new NotFoundException("Offer not found.");

        var slots = await _slotRepository.GetByOfferIdAsync(offerId, cancellationToken);
        return slots
            .Where(s => s.Status != SlotStatus.Cancelled)
            .Select(s => s.ToDto())
            .ToList();
    }

    public async Task<SlotDto> UpdateAsync(Guid id, UpdateSlotDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _slotRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Slot not found.");

        if (dto.EndTime <= dto.StartTime)
            throw new ValidationException("Slot EndTime must be after StartTime.");

        if (dto.Capacity < entity.BookedCount)
            throw new ValidationException("Capacity cannot be less than current booked count.");

        entity.SlotDate = dto.SlotDate;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.Capacity = dto.Capacity;
        entity.Status = dto.Status;
        entity.AvailableCount = Math.Max(0, entity.Capacity - entity.BookedCount);

        if (entity.AvailableCount == 0)
            entity.Status = SlotStatus.Full;

        entity.UpdatedAt = DateTime.UtcNow;

        await _slotRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifySlotUpdatedAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _slotRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Slot not found.");

        entity.Status = SlotStatus.Cancelled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _slotRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifySlotUpdatedAsync(entity, cancellationToken);
    }
}
