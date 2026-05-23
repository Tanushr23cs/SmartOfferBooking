using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOfferBooking.Domain.Entities;

namespace SmartOfferBooking.Infrastructure.Data.Configurations;

public class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.ToTable("waitlist_entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CustomerPhone).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(x => x.Offer)
            .WithMany()
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Slot)
            .WithMany()
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.SlotId, x.CustomerPhone });
        builder.HasIndex(x => x.OfferId);
        builder.HasIndex(x => x.Status);
    }
}
