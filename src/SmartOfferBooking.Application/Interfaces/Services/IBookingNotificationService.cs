using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IBookingNotificationService
{
    Task NotifySlotUpdatedAsync(Slot slot, CancellationToken cancellationToken = default);
    Task NotifyBookingCreatedAsync(Booking booking, CancellationToken cancellationToken = default);
    Task NotifyOfferUpdatedAsync(Offer offer, int totalAvailableSeats = 0, CancellationToken cancellationToken = default);
}
