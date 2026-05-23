using SmartOfferBooking.Application.DTOs.Waitlist;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IWaitlistService
{
    Task<WaitlistDto> CreateAsync(CreateWaitlistDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WaitlistDto>> GetByOfferAsync(Guid offerId, CancellationToken cancellationToken = default);
}
