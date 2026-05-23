using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Infrastructure.Data.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("offers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.OriginalPrice).HasPrecision(18, 2);
        builder.Property(x => x.OfferPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountPercentage).HasPrecision(5, 2);
        builder.Property(x => x.TermsAndConditions).HasMaxLength(5000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasCheckConstraint(
            "CK_Offers_OfferPriceLessThanOriginal",
            "\"OfferPrice\" < \"OriginalPrice\"");

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.StartDate, x.EndDate });
        builder.HasIndex(x => x.Category);
    }
}
