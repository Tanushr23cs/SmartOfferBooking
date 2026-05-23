using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.SignalR;

public class OfferUpdatedEventDto
{
    public Guid OfferId { get; set; }
    public string Title { get; set; } = string.Empty;
    public OfferStatus Status { get; set; }
    public decimal OfferPrice { get; set; }
    public decimal OriginalPrice { get; set; }
    public int TotalAvailableSeats { get; set; }
    public DateTime UpdatedAt { get; set; }
}
