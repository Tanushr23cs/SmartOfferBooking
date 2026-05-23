namespace SmartOfferBooking.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalOffers { get; set; }
    public int ActiveOffers { get; set; }
    public int TotalBookings { get; set; }
    public int TodaysBookings { get; set; }
    public int TotalCapacity { get; set; }
    public int BookedSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal ConversionRate { get; set; }
}
