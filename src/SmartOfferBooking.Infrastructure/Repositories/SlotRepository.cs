using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;
using SmartOfferBooking.Infrastructure.Data;

namespace SmartOfferBooking.Infrastructure.Repositories;

public class SlotRepository : Repository<Slot>, ISlotRepository
{
    public SlotRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Slot>> GetByOfferIdAsync(Guid offerId, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(s => s.OfferId == offerId)
            .OrderBy(s => s.SlotDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

    public async Task<Slot?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<int> GetTotalCapacityAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(s => s.Status != SlotStatus.Cancelled)
            .SumAsync(s => s.Capacity, cancellationToken);

    public async Task<int> GetTotalBookedCountAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(s => s.Status != SlotStatus.Cancelled)
            .SumAsync(s => s.BookedCount, cancellationToken);
}
