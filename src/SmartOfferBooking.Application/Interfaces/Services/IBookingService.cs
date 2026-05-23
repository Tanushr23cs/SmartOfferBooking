using SmartOfferBooking.Application.Common;
using SmartOfferBooking.Application.DTOs.Bookings;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<BookingDto>> GetPagedAsync(BookingQueryDto query, CancellationToken cancellationToken = default);
    Task<BookingDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookingDetailDto> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingDetailDto>> GetRecentDetailsAsync(int limit, CancellationToken cancellationToken = default);
    Task<BookingDto> UpdateStatusAsync(Guid id, UpdateBookingStatusDto dto, CancellationToken cancellationToken = default);
}
