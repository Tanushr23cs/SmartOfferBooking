using SmartOfferBooking.Application.DTOs.Slots;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface ISlotService
{
    Task<SlotDto> CreateAsync(CreateSlotDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SlotDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SlotDto>> GetByOfferIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<SlotDto> UpdateAsync(Guid id, UpdateSlotDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
