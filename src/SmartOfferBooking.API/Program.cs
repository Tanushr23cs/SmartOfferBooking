using Microsoft.EntityFrameworkCore;
using SmartOfferBooking.API.Extensions;
using SmartOfferBooking.API.Hubs;
using SmartOfferBooking.API.Middleware;
using SmartOfferBooking.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

//
// Add services
//
builder.Services.AddApiServices(builder.Configuration);

//
// PostgreSQL / Supabase Database
//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//
// CORS
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.Contains("vercel.app") ||
                origin.Contains("localhost")
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

//
// Global Exception Handling
//
app.UseMiddleware<ExceptionHandlingMiddleware>();

//
// Enable Swagger in ALL environments
//
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Smart Offer Booking API v1"
    );

    c.RoutePrefix = string.Empty;
});

//
// HTTPS only in production
//
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//
// CORS
//
app.UseCors("AllowFrontend");

//
// Authentication & Authorization
//
app.UseAuthentication();
app.UseAuthorization();

//
// Health Check
//
app.MapGet("/health", () =>
    Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow
    }));

//
// Controllers
//
app.MapControllers();

//
// SignalR Hub
//
app.MapHub<BookingHub>("/hubs/booking");

//
// Safe Database Seeding
//
try
{
    await DbSeeder.SeedAsync(app.Services);
    Console.WriteLine("Database seeding completed.");
}
catch (Exception ex)
{
    Console.WriteLine("Database seeding failed.");
    Console.WriteLine(ex.Message);
}

//
// Run App
//
app.Run();