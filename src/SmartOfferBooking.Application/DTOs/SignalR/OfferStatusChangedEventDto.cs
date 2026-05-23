using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.SignalR;

public class OfferStatusChangedEventDto
{
    public Guid OfferId { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }
}
