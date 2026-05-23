using System.ComponentModel.DataAnnotations;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Application.DTOs.Slots;

public class UpdateSlotDto
{
    [Required]
    public DateOnly SlotDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    public SlotStatus Status { get; set; }
}
