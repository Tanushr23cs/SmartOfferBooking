using SmartOfferBooking.Application.DTOs.Offers;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Interfaces.Repositories;

public interface IOfferRepository : IRepository<Offer>
{
    Task<Offer?> GetByIdWithSlotsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Offer> Items, int TotalCount)> GetPagedAsync(
        OfferQueryDto query,
        CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(OfferStatus? status, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> GetAvailableSeatsByOfferIdsAsync(
        IEnumerable<Guid> offerIds,
        CancellationToken cancellationToken = default);
}
