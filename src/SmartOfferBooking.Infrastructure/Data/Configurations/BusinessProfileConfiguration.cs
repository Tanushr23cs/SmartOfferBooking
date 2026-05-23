using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Infrastructure.Data.Configurations;

public class BusinessProfileConfiguration : IEntityTypeConfiguration<BusinessProfile>
{
    public void Configure(EntityTypeBuilder<BusinessProfile> builder)
    {
        builder.ToTable("business_profiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BusinessName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.BusinessType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.OwnerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
    }
}
