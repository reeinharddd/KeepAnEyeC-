using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Microsoft.AspNetCore.SignalR;
using KeepAnEye.Hubs;  // Asegúrate de que esta línea esté presente

namespace KeepAnEye.Services
{
    public class MetricsService
    {
        private readonly IMongoCollection<Metric> _metricsCollection;
        private readonly IHubContext<MetricsHub> _hubContext;

        public MetricsService(IOptions<MongoDbSettings> mongoDbSettings, IHubContext<MetricsHub> hubContext)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _metricsCollection = database.GetCollection<Metric>("metrics");
            _hubContext = hubContext;
        }

        public async Task<Metric> GetLocationAsync(string patientId)
        {
            var filter = Builders<Metric>.Filter.Eq(m => m.PatientId, new ObjectId(patientId));
            var metric = await _metricsCollection.Find(filter).FirstOrDefaultAsync();
            if (metric != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveLocationUpdate", metric);
            }
            return metric;
        }

        public async Task<Metric> UpdateLocationAsync(string patientId, LocationEntry locationEntry)
        {
            var filter = Builders<Metric>.Filter.Eq(m => m.PatientId, new ObjectId(patientId));
            var update = Builders<Metric>.Update.Push(m => m.Location, locationEntry);
            var metric = await _metricsCollection.FindOneAndUpdateAsync(filter, update);
            if (metric != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveLocationUpdate", metric);
            }
            return metric;
        }

        public async Task<List<Metric>> GetMetricsByPatientIdAsync(string patientId)
        {
            var filter = Builders<Metric>.Filter.Eq(m => m.PatientId, new ObjectId(patientId));
            var metrics = await _metricsCollection.Find(filter).ToListAsync();
            if (metrics != null && metrics.Count > 0)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveAllMetrics", metrics);
            }
            return metrics;
        }
    }
}
