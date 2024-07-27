// Hubs/LocationHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KeepAnEye.Hubs
{
    public class LocationHub : Hub
    {
        public async Task SendLocationUpdate(object locationData)
        {
            await Clients.All.SendAsync("ReceiveLocationUpdate", locationData);
        }
    }
}