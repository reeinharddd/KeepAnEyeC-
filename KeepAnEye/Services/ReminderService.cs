using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace KeepAnEye.Services
{
    public class ReminderService
    {
        private readonly IMongoCollection<Reminder> _reminders;

        public ReminderService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _reminders = database.GetCollection<Reminder>("reminders");
        }

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId)
        {
            var filter = Builders<Reminder>.Filter.Eq("UserToRemind", new ObjectId(userId));
            return await _reminders.Find(filter).ToListAsync();
        }

        public async Task<Reminder> GetReminderByIdAsync(string id)
        {
            var filter = Builders<Reminder>.Filter.Eq("Id", new ObjectId(id));
            return await _reminders.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateReminderAsync(Reminder reminder)
        {
            await _reminders.InsertOneAsync(reminder);
        }

        public async Task UpdateReminderAsync(string id, Reminder reminderIn)
        {
            var filter = Builders<Reminder>.Filter.Eq("Id", new ObjectId(id));
            await _reminders.ReplaceOneAsync(filter, reminderIn);
        }

        public async Task DeleteReminderAsync(string id)
        {
            var filter = Builders<Reminder>.Filter.Eq("Id", new ObjectId(id));
            await _reminders.DeleteOneAsync(filter);
        }
    }
}
