using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Interfaces.Repositories;

public interface IAdminUserRepository : IRepository<AdminUser>
{
    Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
