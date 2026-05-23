using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.Helpers;

public static class SlotValidator
{
    public static void EnsureBookable(Slot slot, int peopleCount)
    {
        if (slot.Status is SlotStatus.Closed or SlotStatus.Expired or SlotStatus.Cancelled)
            throw new ValidationException($"Slot is not available. Current status: {slot.Status}.");

        if (slot.Status == SlotStatus.Full || slot.AvailableCount < peopleCount)
            throw new ValidationException("Slot is full or does not have enough capacity.");

        if (slot.BookedCount + peopleCount > slot.Capacity)
            throw new ValidationException("Booking count would exceed slot capacity.");
    }

    public static void RefreshAvailability(Slot slot)
    {
        slot.AvailableCount = Math.Max(0, slot.Capacity - slot.BookedCount);
        slot.Status = slot.AvailableCount <= 0 ? SlotStatus.Full : SlotStatus.Available;
        slot.UpdatedAt = DateTime.UtcNow;
    }
}
