using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Interfaces.Repositories;

public interface ISlotRepository : IRepository<Slot>
{
    Task<IReadOnlyList<Slot>> GetByOfferIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<Slot?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCapacityAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalBookedCountAsync(CancellationToken cancellationToken = default);
}
