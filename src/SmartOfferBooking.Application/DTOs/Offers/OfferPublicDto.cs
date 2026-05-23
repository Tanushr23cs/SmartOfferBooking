using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Offers;

public class OfferPublicDto
{
    public Guid Id { get; set; }
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
    public int MaxBookingPerCustomer { get; set; }
    public string? TermsAndConditions { get; set; }
    public OfferStatus Status { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int AvailableSlotsCount { get; set; }
    public int TotalAvailableSeats { get; set; }
    public DateTime CreatedAt { get; set; }
}
