using SmartOfferBooking.API.Extensions;
using SmartOfferBooking.API.Hubs;
using SmartOfferBooking.API.Middleware;
using SmartOfferBooking.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Offer Booking API v1"));
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapControllers();
app.MapHub<BookingHub>("/hubs/booking");

await DbSeeder.SeedAsync(app.Services);

app.Run();
