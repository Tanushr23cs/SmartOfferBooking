using SmartOfferBooking.Application.DTOs.Bookings;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Interfaces.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<int> CountByOfferAndPhoneAsync(Guid offerId, string phone, CancellationToken cancellationToken = default);
    Task<int> CountTodaysBookingsAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(BookingStatus? status, CancellationToken cancellationToken = default);
    Task<bool> ReferenceExistsAsync(string reference, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetPagedAsync(
        BookingQueryDto query,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetRecentAsync(int limit, CancellationToken cancellationToken = default);
    Task<Booking?> GetByReferenceWithDetailsAsync(string reference, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetRecentWithDetailsAsync(int limit, CancellationToken cancellationToken = default);
}
