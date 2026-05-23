using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Bookings;

public class BookingQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public BookingStatus? Status { get; set; }
    public Guid? OfferId { get; set; }
}
