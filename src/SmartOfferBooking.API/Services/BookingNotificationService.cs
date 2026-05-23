using Microsoft.AspNetCore.SignalR;
using SmartOfferBooking.API.Hubs;
using SmartOfferBooking.Application.DTOs.SignalR;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.API.Services;

public class BookingNotificationService : IBookingNotificationService
{
    private readonly IHubContext<BookingHub> _hubContext;

    public BookingNotificationService(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifySlotUpdatedAsync(Slot slot, CancellationToken cancellationToken = default)
    {
        var payload = new SlotUpdatedEventDto
        {
            SlotId = slot.Id,
            OfferId = slot.OfferId,
            BookedCount = slot.BookedCount,
            AvailableCount = slot.AvailableCount,
            Status = slot.Status,
            IsFull = slot.Status == SlotStatus.Full
        };

        await _hubContext.Clients.Group($"offer-{slot.OfferId}")
            .SendAsync("SlotUpdated", payload, cancellationToken);

        await _hubContext.Clients.Group("admin-dashboard")
            .SendAsync("SlotUpdated", payload, cancellationToken);
    }

    public async Task NotifyBookingCreatedAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var payload = new BookingCreatedEventDto
        {
            BookingId = booking.Id,
            BookingReference = booking.BookingReference,
            OfferId = booking.OfferId,
            SlotId = booking.SlotId,
            CustomerName = booking.CustomerName,
            PeopleCount = booking.PeopleCount,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };

        await _hubContext.Clients.Group($"offer-{booking.OfferId}")
            .SendAsync("BookingCreated", payload, cancellationToken);

        await _hubContext.Clients.Group("admin-dashboard")
            .SendAsync("BookingCreated", payload, cancellationToken);
    }

    public async Task NotifyOfferUpdatedAsync(Offer offer, int totalAvailableSeats = 0, CancellationToken cancellationToken = default)
    {
        var payload = new OfferUpdatedEventDto
        {
            OfferId = offer.Id,
            Title = offer.Title,
            Status = offer.Status,
            OfferPrice = offer.OfferPrice,
            OriginalPrice = offer.OriginalPrice,
            TotalAvailableSeats = totalAvailableSeats,
            UpdatedAt = DateTime.UtcNow
        };

        await _hubContext.Clients.Group($"offer-{offer.Id}")
            .SendAsync("OfferUpdated", payload, cancellationToken);

        await _hubContext.Clients.All
            .SendAsync("OfferUpdated", payload, cancellationToken);

        await _hubContext.Clients.Group("admin-dashboard")
            .SendAsync("OfferUpdated", payload, cancellationToken);
    }
}
