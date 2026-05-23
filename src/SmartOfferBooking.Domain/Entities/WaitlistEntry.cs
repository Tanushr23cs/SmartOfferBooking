using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Domain.Entities;

public class WaitlistEntry : BaseEntity
{
    public Guid OfferId { get; set; }
    public Guid SlotId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public int PeopleCount { get; set; } = 1;
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Waiting;

    public Offer Offer { get; set; } = null!;
    public Slot Slot { get; set; } = null!;
}
