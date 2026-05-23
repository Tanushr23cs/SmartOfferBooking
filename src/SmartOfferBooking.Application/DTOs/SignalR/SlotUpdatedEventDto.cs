using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.SignalR;

public class SlotUpdatedEventDto
{
    public Guid SlotId { get; set; }
    public Guid OfferId { get; set; }
    public int BookedCount { get; set; }
    public int AvailableCount { get; set; }
    public SlotStatus Status { get; set; }
    public bool IsFull { get; set; }
}
