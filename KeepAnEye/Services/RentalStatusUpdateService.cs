using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KeepAnEye.Services
{
    public class RentalStatusUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Se ejecuta cada 24 horas

        public RentalStatusUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var rentalService = scope.ServiceProvider.GetRequiredService<RentalService>();
                    await rentalService.UpdateRentalStatusAsync();
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
