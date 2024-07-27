using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace KeepAnEye.Services
{
    public class PaymentService
    {
        private readonly IMongoCollection<Payments> _payments;

        public PaymentService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _payments = mongoDatabase.GetCollection<Payments>("payments");
        }

        public async Task<string> CreatePaymentAsync(Payments newPayment)
        {
            await _payments.InsertOneAsync(newPayment);
            return newPayment.Id;
        }
    }
}
