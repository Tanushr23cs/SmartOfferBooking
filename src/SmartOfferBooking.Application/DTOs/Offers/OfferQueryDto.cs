using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Offers;

public class OfferQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? BusinessType { get; set; }
    public OfferStatus? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool AvailableOnly { get; set; }
    public bool PublicOnly { get; set; }
    public DateOnly? SlotDate { get; set; }
}
