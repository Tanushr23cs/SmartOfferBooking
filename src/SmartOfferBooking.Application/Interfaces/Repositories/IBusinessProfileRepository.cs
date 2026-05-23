using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Interfaces.Repositories;

public interface IBusinessProfileRepository : IRepository<BusinessProfile>
{
    Task<BusinessProfile?> GetFirstAsync(CancellationToken cancellationToken = default);
}
