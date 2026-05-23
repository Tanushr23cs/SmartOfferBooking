using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Helpers;

public static class OfferValidator
{
    public static void ValidatePricing(decimal originalPrice, decimal offerPrice)
    {
        if (offerPrice >= originalPrice)
            throw new ValidationException("OfferPrice must be less than OriginalPrice.");
    }

    public static decimal CalculateDiscountPercentage(decimal originalPrice, decimal offerPrice)
    {
        if (originalPrice <= 0) return 0;
        return Math.Round((originalPrice - offerPrice) / originalPrice * 100, 2);
    }

    public static void EnsureBookable(Offer offer)
    {
        if (offer.Status == OfferStatus.Expired)
            throw new ValidationException("Expired offers cannot be booked.");

        if (offer.Status == OfferStatus.Cancelled)
            throw new ValidationException("Cancelled offers are not available for booking.");

        if (offer.Status != OfferStatus.Active)
            throw new ValidationException("Only active offers can be booked.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (offer.EndDate < today)
            throw new ValidationException("This offer has expired.");
    }

    public static OfferStatus ResolveStatus(Offer offer)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (offer.Status == OfferStatus.Cancelled)
            return OfferStatus.Cancelled;

        if (offer.EndDate < today && offer.Status != OfferStatus.Draft)
            return OfferStatus.Expired;

        return offer.Status;
    }
}
