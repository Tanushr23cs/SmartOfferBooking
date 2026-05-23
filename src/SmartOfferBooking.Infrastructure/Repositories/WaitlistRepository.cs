using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;
using SmartOfferBooking.Infrastructure.Data;

namespace SmartOfferBooking.Infrastructure.Repositories;

public class WaitlistRepository : Repository<WaitlistEntry>, IWaitlistRepository
{
    public WaitlistRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> CountBySlotAndPhoneAsync(
        Guid slotId,
        string phone,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().CountAsync(
            w => w.SlotId == slotId &&
                 w.CustomerPhone == phone &&
                 w.Status == WaitlistStatus.Waiting,
            cancellationToken);
}
