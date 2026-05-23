using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOfferBooking.Domain.Entities;
namespace SmartOfferBooking.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookingReference).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CustomerPhone).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(200);
        builder.Property(x => x.SpecialNote).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(x => x.Offer)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Slot)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.BookingReference).IsUnique();
        builder.HasIndex(x => new { x.OfferId, x.CustomerPhone });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
    }
}
