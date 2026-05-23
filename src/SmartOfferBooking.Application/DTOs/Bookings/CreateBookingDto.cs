using System.ComponentModel.DataAnnotations;

namespace SmartOfferBooking.Application.DTOs.Bookings;

public class CreateBookingDto
{
    [Required]
    public Guid OfferId { get; set; }

    [Required]
    public Guid SlotId { get; set; }

    [Required, MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, Phone, MaxLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;

    [EmailAddress, MaxLength(200)]
    public string? CustomerEmail { get; set; }

    [Range(1, 50)]
    public int PeopleCount { get; set; } = 1;

    [MaxLength(1000)]
    public string? SpecialNote { get; set; }
}
