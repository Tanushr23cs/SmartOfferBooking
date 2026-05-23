using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Slots;

public class SlotDto
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public DateOnly SlotDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Capacity { get; set; }
    public int BookedCount { get; set; }
    public int AvailableCount { get; set; }
    public SlotStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
