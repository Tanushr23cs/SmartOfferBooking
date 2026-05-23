using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Domain.Entities;

public class Offer : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal OfferPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int TotalCapacity { get; set; }
    public int MaxBookingPerCustomer { get; set; } = 1;
    public string? TermsAndConditions { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Draft;

    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
