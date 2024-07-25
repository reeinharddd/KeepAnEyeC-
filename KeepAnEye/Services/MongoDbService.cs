// Services/MongoDbService.cs
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KeepAnEye.Services
{
    public class MongoDbService
    {
        public MongoClient Client { get; }

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            Client = new MongoClient(settings.ConnectionString);
        }
    }

    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}
