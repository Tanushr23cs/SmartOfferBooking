using SmartOfferBooking.Application.DTOs.Auth;
using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Application.Interfaces;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;

namespace SmartOfferBooking.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IAdminUserRepository adminUserRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _adminUserRepository = adminUserRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _adminUserRepository.GetByEmailAsync(email, cancellationToken)
            ?? await _adminUserRepository.GetByUsernameAsync(email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is disabled.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _adminUserRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}
