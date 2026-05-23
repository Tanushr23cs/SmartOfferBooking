using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SmartOfferBooking.API.Hubs;

public class BookingHub : Hub
{
    public async Task JoinOfferGroup(string offerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"offer-{offerId}");
    }

    public async Task LeaveOfferGroup(string offerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"offer-{offerId}");
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task JoinAdminDashboard()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admin-dashboard");
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task LeaveAdminDashboard()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin-dashboard");
    }
}
