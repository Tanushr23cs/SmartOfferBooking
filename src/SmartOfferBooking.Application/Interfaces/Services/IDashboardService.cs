using SmartOfferBooking.Application.DTOs.Dashboard;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
