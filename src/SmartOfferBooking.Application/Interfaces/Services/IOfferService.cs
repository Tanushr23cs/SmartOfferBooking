using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Offers;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IOfferService
{
    Task<OfferDto> CreateAsync(CreateOfferDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<OfferDto>> GetPagedAsync(OfferQueryDto query, CancellationToken cancellationToken = default);
    Task<PagedResult<OfferPublicDto>> GetPublicPagedAsync(OfferQueryDto query, CancellationToken cancellationToken = default);
    Task<OfferDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OfferPublicDto> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OfferDto> UpdateAsync(Guid id, UpdateOfferDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
