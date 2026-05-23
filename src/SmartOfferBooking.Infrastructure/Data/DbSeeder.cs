using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartOfferBooking.Domain.Entities;
using SmartOfferBooking.Domain.Enums;

namespace SmartOfferBooking.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DbSeeder");

        await context.Database.MigrateAsync();

        if (!await context.AdminUsers.AnyAsync())
        {
            var admin = new AdminUser
            {
                Username = "admin",
                Email = "admin@smartoffer.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.SuperAdmin,
                IsActive = true
            };

            context.AdminUsers.Add(admin);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default admin user: admin / Admin@123");
        }
    }
}
