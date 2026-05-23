using System.ComponentModel.DataAnnotations;

namespace SmartOfferBooking.Application.DTOs.Business;

public class UpdateBusinessProfileDto
{
    [Required, MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string BusinessType { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string OwnerName { get; set; } = string.Empty;

    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [Required]
    public TimeOnly OpeningTime { get; set; }

    [Required]
    public TimeOnly ClosingTime { get; set; }
}
