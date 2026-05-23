using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Infrastructure.Data.Configurations;

public class SlotConfiguration : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder.ToTable("slots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(x => x.Offer)
            .WithMany(x => x.Slots)
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasCheckConstraint("CK_Slots_BookedCount", "\"BookedCount\" >= 0");
        builder.HasCheckConstraint("CK_Slots_AvailableCount", "\"AvailableCount\" >= 0");
        builder.HasCheckConstraint("CK_Slots_Capacity", "\"Capacity\" > 0");

        builder.HasIndex(x => x.OfferId);
        builder.HasIndex(x => new { x.OfferId, x.SlotDate, x.StartTime });
        builder.HasIndex(x => x.Status);
    }
}
