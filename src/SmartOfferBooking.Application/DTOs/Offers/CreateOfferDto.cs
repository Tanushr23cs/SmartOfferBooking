using System.ComponentModel.DataAnnotations;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Offers;

public class CreateOfferDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal OriginalPrice { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal OfferPrice { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalCapacity { get; set; }

    [Range(1, 100)]
    public int MaxBookingPerCustomer { get; set; } = 1;

    [MaxLength(5000)]
    public string? TermsAndConditions { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Draft;
}
