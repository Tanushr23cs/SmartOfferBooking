using SmartOfferBooking.Application.DTOs.Auth;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
