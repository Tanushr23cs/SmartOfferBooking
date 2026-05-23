using SmartOfferBooking.Application.DTOs.Dashboard;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ISlotRepository _slotRepository;

    public DashboardService(
        IOfferRepository offerRepository,
        IBookingRepository bookingRepository,
        ISlotRepository slotRepository)
    {
        _offerRepository = offerRepository;
        _bookingRepository = bookingRepository;
        _slotRepository = slotRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalOffers = await _offerRepository.CountByStatusAsync(null, cancellationToken);
        var activeOffers = await _offerRepository.CountByStatusAsync(OfferStatus.Active, cancellationToken);
        var totalBookings = await _bookingRepository.CountByStatusAsync(null, cancellationToken);
        var todaysBookings = await _bookingRepository.CountTodaysBookingsAsync(cancellationToken);
        var totalCapacity = await _slotRepository.GetTotalCapacityAsync(cancellationToken);
        var bookedSeats = await _slotRepository.GetTotalBookedCountAsync(cancellationToken);
        var availableSeats = Math.Max(0, totalCapacity - bookedSeats);

        var conversionRate = totalCapacity > 0
            ? Math.Round((decimal)bookedSeats / totalCapacity * 100, 2)
            : 0m;

        return new DashboardSummaryDto
        {
            TotalOffers = totalOffers,
            ActiveOffers = activeOffers,
            TotalBookings = totalBookings,
            TodaysBookings = todaysBookings,
            TotalCapacity = totalCapacity,
            BookedSeats = bookedSeats,
            AvailableSeats = availableSeats,
            ConversionRate = conversionRate
        };
    }
}
