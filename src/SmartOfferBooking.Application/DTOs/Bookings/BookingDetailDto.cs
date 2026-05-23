using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Bookings;

public class BookingDetailDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid OfferId { get; set; }
    public Guid SlotId { get; set; }
    public string OfferTitle { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public int PeopleCount { get; set; }
    public string? SpecialNote { get; set; }
    public BookingStatus Status { get; set; }
    public DateOnly SlotDate { get; set; }
    public TimeOnly SlotStartTime { get; set; }
    public TimeOnly SlotEndTime { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
