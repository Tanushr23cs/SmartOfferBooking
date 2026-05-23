namespace SmartOfferBooking.Application.Helpers;

public static class BookingReferenceGenerator
{
    public static string Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"BK-{datePart}-{randomPart}";
    }
}
