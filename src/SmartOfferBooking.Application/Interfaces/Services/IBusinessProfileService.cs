using SmartOfferBooking.Application.DTOs.Business;

namespace SmartOfferBooking.Application.Interfaces.Services;

public interface IBusinessProfileService
{
    Task<BusinessProfileDto> CreateAsync(CreateBusinessProfileDto dto, CancellationToken cancellationToken = default);
    Task<BusinessProfileDto?> GetAsync(CancellationToken cancellationToken = default);
    Task<BusinessProfileDto> UpdateAsync(Guid id, UpdateBusinessProfileDto dto, CancellationToken cancellationToken = default);
}
