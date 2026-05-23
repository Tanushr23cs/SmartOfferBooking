using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.SignalR;

public class BookingCreatedEventDto
{
    public Guid BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid OfferId { get; set; }
    public Guid SlotId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int PeopleCount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
