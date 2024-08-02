// Services/MongoDbService.cs
using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;

namespace KeepAnEye.Services
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;
       
        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

        }
    }

    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}
