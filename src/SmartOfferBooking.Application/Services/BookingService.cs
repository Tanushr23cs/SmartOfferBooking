using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Bookings;
using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Application.Helpers;
using SmartOfferBooking.Application.Interfaces;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Application.Mappings;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingNotificationService _notificationService;
    private readonly IBusinessProfileRepository _businessProfileRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IOfferRepository offerRepository,
        ISlotRepository slotRepository,
        IBusinessProfileRepository businessProfileRepository,
        IUnitOfWork unitOfWork,
        IBookingNotificationService notificationService)
    {
        _bookingRepository = bookingRepository;
        _offerRepository = offerRepository;
        _slotRepository = slotRepository;
        _businessProfileRepository = businessProfileRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        BookingDto? result = null;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var offer = await _offerRepository.GetByIdAsync(dto.OfferId, cancellationToken)
                ?? throw new NotFoundException("Offer not found.");

            offer.Status = OfferValidator.ResolveStatus(offer);
            OfferValidator.EnsureBookable(offer);

            var slot = await _slotRepository.GetByIdForUpdateAsync(dto.SlotId, cancellationToken)
                ?? throw new NotFoundException("Slot not found.");

            if (slot.OfferId != offer.Id)
                throw new ValidationException("Slot does not belong to the specified offer.");

            SlotValidator.EnsureBookable(slot, dto.PeopleCount);

            var phone = dto.CustomerPhone.Trim();
            var existingBookings = await _bookingRepository.CountByOfferAndPhoneAsync(
                offer.Id,
                phone,
                cancellationToken);

            if (existingBookings >= offer.MaxBookingPerCustomer)
                throw new ValidationException(
                    $"Phone number has reached the maximum booking limit ({offer.MaxBookingPerCustomer}) for this offer.");

            var reference = await GenerateUniqueReferenceAsync(cancellationToken);

            var booking = new Booking
            {
                BookingReference = reference,
                OfferId = offer.Id,
                SlotId = slot.Id,
                CustomerName = dto.CustomerName.Trim(),
                CustomerPhone = phone,
                CustomerEmail = dto.CustomerEmail?.Trim().ToLowerInvariant(),
                PeopleCount = dto.PeopleCount,
                SpecialNote = dto.SpecialNote,
                Status = BookingStatus.Confirmed
            };

            slot.BookedCount += dto.PeopleCount;
            SlotValidator.RefreshAvailability(slot);

            await _bookingRepository.AddAsync(booking, cancellationToken);
            await _slotRepository.UpdateAsync(slot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyBookingCreatedAsync(booking, cancellationToken);
            await _notificationService.NotifySlotUpdatedAsync(slot, cancellationToken);

            result = booking.ToDto();
        }, cancellationToken);

        return result!;
    }

    public async Task<PagedResult<BookingDto>> GetPagedAsync(BookingQueryDto query, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _bookingRepository.GetPagedAsync(query, cancellationToken);
        return new PagedResult<BookingDto>
        {
            Items = items.Select(b => b.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    public async Task<IReadOnlyList<BookingDetailDto>> GetRecentDetailsAsync(int limit, CancellationToken cancellationToken = default)
    {
        var business = await _businessProfileRepository.GetFirstAsync(cancellationToken);
        var bookings = await _bookingRepository.GetRecentWithDetailsAsync(limit, cancellationToken);
        return bookings.Select(b => b.ToDetailDto(business)).ToList();
    }

    public async Task<BookingDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _bookingRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");
        return entity.ToDto();
    }

    public async Task<BookingDetailDto> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        var entity = await _bookingRepository.GetByReferenceWithDetailsAsync(reference, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");
        var business = await _businessProfileRepository.GetFirstAsync(cancellationToken);
        return entity.ToDetailDto(business);
    }

    public async Task<BookingDto> UpdateStatusAsync(Guid id, UpdateBookingStatusDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _bookingRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        var previousStatus = entity.Status;
        entity.Status = dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == BookingStatus.Cancelled && previousStatus != BookingStatus.Cancelled)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var slot = await _slotRepository.GetByIdForUpdateAsync(entity.SlotId, cancellationToken)
                    ?? throw new NotFoundException("Slot not found.");

                slot.BookedCount = Math.Max(0, slot.BookedCount - entity.PeopleCount);
                SlotValidator.RefreshAvailability(slot);
                await _slotRepository.UpdateAsync(slot, cancellationToken);
                await _bookingRepository.UpdateAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _notificationService.NotifySlotUpdatedAsync(slot, cancellationToken);
            }, cancellationToken);
        }
        else
        {
            await _bookingRepository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return entity.ToDto();
    }

    private async Task<string> GenerateUniqueReferenceAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var reference = BookingReferenceGenerator.Generate();
            if (!await _bookingRepository.ReferenceExistsAsync(reference, cancellationToken))
                return reference;
        }

        throw new AppException("Unable to generate a unique booking reference.", 500);
    }
}
