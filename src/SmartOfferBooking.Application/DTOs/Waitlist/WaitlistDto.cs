using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Waitlist;

public class WaitlistDto
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public Guid SlotId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public int PeopleCount { get; set; }
    public WaitlistStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
