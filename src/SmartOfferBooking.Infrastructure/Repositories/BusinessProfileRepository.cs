using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Infrastructure.Data;

namespace SmartOfferBooking.Infrastructure.Repositories;

public class BusinessProfileRepository : Repository<BusinessProfile>, IBusinessProfileRepository
{
    public BusinessProfileRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<BusinessProfile?> GetFirstAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().OrderBy(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);
}
