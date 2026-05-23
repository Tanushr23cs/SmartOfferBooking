using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(AdminUser user);
}
