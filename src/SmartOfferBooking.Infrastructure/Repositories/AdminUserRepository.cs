using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Infrastructure.Data;

namespace SmartOfferBooking.Infrastructure.Repositories;

public class AdminUserRepository : Repository<AdminUser>, IAdminUserRepository
{
    public AdminUserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
}
