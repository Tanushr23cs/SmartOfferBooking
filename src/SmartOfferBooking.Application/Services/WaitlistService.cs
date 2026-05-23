using SmartOfferBooking.Application.DTOs.Waitlist;
using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Application.Helpers;
using SmartOfferBooking.Application.Interfaces;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Services;

public class WaitlistService : IWaitlistService
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WaitlistService(
        IWaitlistRepository waitlistRepository,
        IOfferRepository offerRepository,
        ISlotRepository slotRepository,
        IUnitOfWork unitOfWork)
    {
        _waitlistRepository = waitlistRepository;
        _offerRepository = offerRepository;
        _slotRepository = slotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WaitlistDto> CreateAsync(CreateWaitlistDto dto, CancellationToken cancellationToken = default)
    {
        var offer = await _offerRepository.GetByIdAsync(dto.OfferId, cancellationToken)
            ?? throw new NotFoundException("Offer not found.");

        offer.Status = OfferValidator.ResolveStatus(offer);
        OfferValidator.EnsureBookable(offer);

        var slot = await _slotRepository.GetByIdAsync(dto.SlotId, cancellationToken)
            ?? throw new NotFoundException("Slot not found.");

        if (slot.OfferId != offer.Id)
            throw new ValidationException("Slot does not belong to this offer.");

        if (slot.Status != SlotStatus.Full && slot.AvailableCount >= dto.PeopleCount)
            throw new ValidationException("Slot still has availability. Please book directly.");

        var phone = dto.CustomerPhone.Trim();
        var existing = await _waitlistRepository.CountBySlotAndPhoneAsync(slot.Id, phone, cancellationToken);
        if (existing > 0)
            throw new ConflictException("You are already on the waitlist for this slot.");

        var entry = new WaitlistEntry
        {
            OfferId = offer.Id,
            SlotId = slot.Id,
            CustomerName = dto.CustomerName.Trim(),
            CustomerPhone = phone,
            CustomerEmail = dto.CustomerEmail?.Trim().ToLowerInvariant(),
            PeopleCount = dto.PeopleCount,
            Status = WaitlistStatus.Waiting
        };

        await _waitlistRepository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entry);
    }

    public async Task<IReadOnlyList<WaitlistDto>> GetByOfferAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var items = await _waitlistRepository.FindAsync(w => w.OfferId == offerId, cancellationToken);
        return items.Select(Map).ToList();
    }

    private static WaitlistDto Map(WaitlistEntry e) => new()
    {
        Id = e.Id,
        OfferId = e.OfferId,
        SlotId = e.SlotId,
        CustomerName = e.CustomerName,
        CustomerPhone = e.CustomerPhone,
        CustomerEmail = e.CustomerEmail,
        PeopleCount = e.PeopleCount,
        Status = e.Status,
        CreatedAt = e.CreatedAt
    };
}
