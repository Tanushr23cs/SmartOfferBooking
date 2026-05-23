using System.ComponentModel.DataAnnotations;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Bookings;

public class UpdateBookingStatusDto
{
    [Required]
    public BookingStatus Status { get; set; }
}
