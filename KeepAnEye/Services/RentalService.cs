using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeepAnEye.Services
{
    public class RentalService
    {
        private readonly IMongoCollection<Rental> _rental;

        public RentalService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _rental = mongoDatabase.GetCollection<Rental>("rental");
        }

        public async Task CreateRentalAsync(Rental newRental)
        {
            newRental.StartDate = DateTime.UtcNow;
            newRental.EndDate = newRental.StartDate.AddDays(30);
            newRental.Status = "active";

            await _rental.InsertOneAsync(newRental);
        }

        public async Task UpdateRentalStatusAsync()
        {
            var filter = Builders<Rental>.Filter.And(
                Builders<Rental>.Filter.Lt(r => r.EndDate, DateTime.UtcNow),
                Builders<Rental>.Filter.Eq(r => r.Status, "active")
            );

            var update = Builders<Rental>.Update.Set(r => r.Status, "pending");

            await _rental.UpdateManyAsync(filter, update);
        }
    }
}
