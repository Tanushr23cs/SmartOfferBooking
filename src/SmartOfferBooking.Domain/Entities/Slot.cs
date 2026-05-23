using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Domain.Entities;

public class Slot : BaseEntity
{
    public Guid OfferId { get; set; }
    public DateOnly SlotDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Capacity { get; set; }
    public int BookedCount { get; set; }
    public int AvailableCount { get; set; }
    public SlotStatus Status { get; set; } = SlotStatus.Available;

    public Offer Offer { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
