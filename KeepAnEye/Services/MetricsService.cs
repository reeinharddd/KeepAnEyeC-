using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace KeepAnEye.Services
{
    public class MetricsService
    {
        private readonly IMongoCollection<Metric> _metricsCollection;

        public MetricsService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _metricsCollection = database.GetCollection<Metric>("metrics");
        }

        public async Task<Metric> GetLocationAsync(string patientId)
        {
            var filter = Builders<Metric>.Filter.Eq(m => m.PatientId, new ObjectId(patientId));
            return await _metricsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Metric> UpdateLocationAsync(string patientId, LocationEntry locationEntry)
        {
            var filter = Builders<Metric>.Filter.Eq(m => m.PatientId, new ObjectId(patientId));
            var update = Builders<Metric>.Update.Push(m => m.Location, locationEntry);
            return await _metricsCollection.FindOneAndUpdateAsync(filter, update);
        }
        // En MetricsService.cs

        public async Task<List<Metric>> GetMetricsByPatientIdAsync(string patientId)
        {
            var filter = Builders<Metric>.Filter.Eq(m => m.PatientId, new ObjectId(patientId));
            return await _metricsCollection.Find(filter).ToListAsync();
        }

    }
}
