using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.Application.DTOs.Bookings;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;
using SmartOfferBooking.Infrastructure.Data;

namespace SmartOfferBooking.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Booking?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().FirstOrDefaultAsync(b => b.BookingReference == reference, cancellationToken);

    public async Task<int> CountByOfferAndPhoneAsync(
        Guid offerId,
        string phone,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().CountAsync(
            b => b.OfferId == offerId && b.CustomerPhone == phone && b.Status != BookingStatus.Cancelled,
            cancellationToken);

    public async Task<int> CountTodaysBookingsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        return await DbSet.AsNoTracking().CountAsync(
            b => b.CreatedAt >= today && b.CreatedAt < tomorrow,
            cancellationToken);
    }

    public async Task<int> CountByStatusAsync(BookingStatus? status, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();
        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> ReferenceExistsAsync(string reference, CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(b => b.BookingReference == reference, cancellationToken);

    public async Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetPagedAsync(
        BookingQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var q = DbSet.AsNoTracking().AsQueryable();

        if (query.Status.HasValue)
            q = q.Where(b => b.Status == query.Status.Value);

        if (query.OfferId.HasValue)
            q = q.Where(b => b.OfferId == query.OfferId.Value);

        var total = await q.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await q
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Booking>> GetRecentAsync(int limit, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .Take(Math.Clamp(limit, 1, 50))
            .ToListAsync(cancellationToken);

    public async Task<Booking?> GetByReferenceWithDetailsAsync(string reference, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Include(b => b.Offer)
            .Include(b => b.Slot)
            .FirstOrDefaultAsync(b => b.BookingReference == reference, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetRecentWithDetailsAsync(int limit, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Include(b => b.Offer)
            .Include(b => b.Slot)
            .OrderByDescending(b => b.CreatedAt)
            .Take(Math.Clamp(limit, 1, 50))
            .ToListAsync(cancellationToken);
}
