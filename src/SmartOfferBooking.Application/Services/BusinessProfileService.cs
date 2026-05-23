using SmartOfferBooking.Application.DTOs.Business;
using SmartOfferBooking.Application.Exceptions;
using SmartOfferBooking.Application.Interfaces;
using SmartOfferBooking.Application.Interfaces.Repositories;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Application.Mappings;
using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Application.Services;

public class BusinessProfileService : IBusinessProfileService
{
    private readonly IBusinessProfileRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public BusinessProfileService(IBusinessProfileRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessProfileDto> CreateAsync(CreateBusinessProfileDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetFirstAsync(cancellationToken);
        if (existing is not null)
            throw new ConflictException("A business profile already exists. Use update instead.");

        var entity = new BusinessProfile
        {
            BusinessName = dto.BusinessName.Trim(),
            BusinessType = dto.BusinessType.Trim(),
            OwnerName = dto.OwnerName.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            Address = dto.Address.Trim(),
            City = dto.City.Trim(),
            LogoUrl = dto.LogoUrl,
            OpeningTime = dto.OpeningTime,
            ClosingTime = dto.ClosingTime
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task<BusinessProfileDto?> GetAsync(CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetFirstAsync(cancellationToken);
        return entity?.ToDto();
    }

    public async Task<BusinessProfileDto> UpdateAsync(Guid id, UpdateBusinessProfileDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Business profile not found.");

        entity.BusinessName = dto.BusinessName.Trim();
        entity.BusinessType = dto.BusinessType.Trim();
        entity.OwnerName = dto.OwnerName.Trim();
        entity.Phone = dto.Phone.Trim();
        entity.Email = dto.Email.Trim().ToLowerInvariant();
        entity.Address = dto.Address.Trim();
        entity.City = dto.City.Trim();
        entity.LogoUrl = dto.LogoUrl;
        entity.OpeningTime = dto.OpeningTime;
        entity.ClosingTime = dto.ClosingTime;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }
}
