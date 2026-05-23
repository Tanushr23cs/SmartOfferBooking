using System.ComponentModel.DataAnnotations;

namespace SmartOfferBooking.Application.DTOs.Slots;

public class CreateSlotDto
{
    [Required]
    public Guid OfferId { get; set; }

    [Required]
    public DateOnly SlotDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
}
