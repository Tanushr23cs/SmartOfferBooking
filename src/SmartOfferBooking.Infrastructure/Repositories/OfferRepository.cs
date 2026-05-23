using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.Application.DTOs.Offers;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;
using SmartOfferBooking.Infrastructure.Data;

namespace SmartOfferBooking.Infrastructure.Repositories;

public class OfferRepository : Repository<Offer>, IOfferRepository
{
    public OfferRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Offer?> GetByIdWithSlotsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet.Include(x => x.Slots).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Offer> Items, int TotalCount)> GetPagedAsync(
        OfferQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(query.BusinessType))
        {
            var business = await Context.BusinessProfiles.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (business is null || !business.BusinessType.Equals(query.BusinessType, StringComparison.OrdinalIgnoreCase))
                return (Array.Empty<Offer>(), 0);
        }

        var q = DbSet.AsNoTracking().AsQueryable();

        if (query.PublicOnly)
        {
            q = q.Where(o => o.Status == OfferStatus.Active && o.EndDate >= today);
        }
        else if (query.Status.HasValue)
        {
            q = q.Where(o => o.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(o =>
                o.Title.ToLower().Contains(term) ||
                o.Description.ToLower().Contains(term) ||
                o.Category.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
            q = q.Where(o => o.Category == query.Category);

        if (query.MinPrice.HasValue)
            q = q.Where(o => o.OfferPrice >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            q = q.Where(o => o.OfferPrice <= query.MaxPrice.Value);

        if (query.AvailableOnly)
        {
            q = q.Where(o => Context.Slots.Any(s =>
                s.OfferId == o.Id &&
                s.Status == SlotStatus.Available &&
                s.AvailableCount > 0));
        }

        if (query.SlotDate.HasValue)
        {
            var date = query.SlotDate.Value;
            q = q.Where(o => Context.Slots.Any(s =>
                s.OfferId == o.Id &&
                s.SlotDate == date &&
                s.Status != SlotStatus.Cancelled));
        }

        var total = await q.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<int> CountByStatusAsync(OfferStatus? status, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetAvailableSeatsByOfferIdsAsync(
        IEnumerable<Guid> offerIds,
        CancellationToken cancellationToken = default)
    {
        var ids = offerIds.ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, int>();

        return await Context.Slots.AsNoTracking()
            .Where(s => ids.Contains(s.OfferId) && s.Status != SlotStatus.Cancelled)
            .GroupBy(s => s.OfferId)
            .Select(g => new { OfferId = g.Key, Total = g.Sum(s => s.AvailableCount) })
            .ToDictionaryAsync(x => x.OfferId, x => x.Total, cancellationToken);
    }
}
