using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Domain.Entities;

public class AdminUser : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Admin;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
