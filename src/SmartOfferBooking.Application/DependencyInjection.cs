using Microsoft.Extensions.DependencyInjection;
using SmartOfferBooking.Application.Interfaces.Services;
using SmartOfferBooking.Application.Services;

namespace SmartOfferBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBusinessProfileService, BusinessProfileService>();
        services.AddScoped<IOfferService, OfferService>();
        services.AddScoped<ISlotService, SlotService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IWaitlistService, WaitlistService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
