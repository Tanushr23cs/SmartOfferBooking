using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Interfaces.Repositories;

public interface IWaitlistRepository : IRepository<WaitlistEntry>
{
    Task<int> CountBySlotAndPhoneAsync(Guid slotId, string phone, CancellationToken cancellationToken = default);
}
