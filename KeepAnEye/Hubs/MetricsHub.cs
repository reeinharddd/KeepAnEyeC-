// Hubs/MetricsHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KeepAnEye.Hubs
{
    public class MetricsHub : Hub
    {
        public async Task SendLocationUpdate(object locationData)
        {
            await Clients.All.SendAsync("ReceiveLocationUpdate", locationData);
        }

        public async Task SendMetricUpdate(object metricData)
        {
            await Clients.All.SendAsync("ReceiveMetricUpdate", metricData);
        }
    }
}