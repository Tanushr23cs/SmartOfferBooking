using SmartOfferBooking.Application.DTOs.Bookings;
using SmartOfferBooking.Application.DTOs.Business;
using SmartOfferBooking.Application.DTOs.Offers;
using SmartOfferBooking.Application.DTOs.Slots;
using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Mappings;

public static class EntityMapper
{
    public static BusinessProfileDto ToDto(this BusinessProfile entity) => new()
    {
        Id = entity.Id,
        BusinessName = entity.BusinessName,
        BusinessType = entity.BusinessType,
        OwnerName = entity.OwnerName,
        Phone = entity.Phone,
        Email = entity.Email,
        Address = entity.Address,
        City = entity.City,
        LogoUrl = entity.LogoUrl,
        OpeningTime = entity.OpeningTime,
        ClosingTime = entity.ClosingTime,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static OfferDto ToDto(this Offer entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        Category = entity.Category,
        OriginalPrice = entity.OriginalPrice,
        OfferPrice = entity.OfferPrice,
        DiscountPercentage = entity.DiscountPercentage,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime,
        TotalCapacity = entity.TotalCapacity,
        MaxBookingPerCustomer = entity.MaxBookingPerCustomer,
        TermsAndConditions = entity.TermsAndConditions,
        Status = entity.Status,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static SlotDto ToDto(this Slot entity) => new()
    {
        Id = entity.Id,
        OfferId = entity.OfferId,
        SlotDate = entity.SlotDate,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime,
        Capacity = entity.Capacity,
        BookedCount = entity.BookedCount,
        AvailableCount = entity.AvailableCount,
        Status = entity.Status,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static BookingDetailDto ToDetailDto(this Booking entity, BusinessProfile? business = null) => new()
    {
        Id = entity.Id,
        BookingReference = entity.BookingReference,
        OfferId = entity.OfferId,
        SlotId = entity.SlotId,
        OfferTitle = entity.Offer?.Title ?? string.Empty,
        BusinessName = business?.BusinessName ?? "Smart Offer Partner",
        CustomerName = entity.CustomerName,
        CustomerPhone = entity.CustomerPhone,
        CustomerEmail = entity.CustomerEmail,
        PeopleCount = entity.PeopleCount,
        SpecialNote = entity.SpecialNote,
        Status = entity.Status,
        SlotDate = entity.Slot?.SlotDate ?? default,
        SlotStartTime = entity.Slot?.StartTime ?? default,
        SlotEndTime = entity.Slot?.EndTime ?? default,
        Address = business?.Address,
        City = business?.City,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static BookingDto ToDto(this Booking entity) => new()
    {
        Id = entity.Id,
        BookingReference = entity.BookingReference,
        OfferId = entity.OfferId,
        SlotId = entity.SlotId,
        CustomerName = entity.CustomerName,
        CustomerPhone = entity.CustomerPhone,
        CustomerEmail = entity.CustomerEmail,
        PeopleCount = entity.PeopleCount,
        SpecialNote = entity.SpecialNote,
        Status = entity.Status,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
