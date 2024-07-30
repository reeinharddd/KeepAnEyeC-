using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeepAnEye.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _usersCollection = mongoDatabase.GetCollection<User>("users");
        }

        public async Task CreateUserAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _usersCollection.Find(user => true).ToListAsync();
        }

        public async Task<User> GetUserAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null;
            }

            return await _usersCollection.Find(user => user.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(string id, User userIn)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new ArgumentException("Invalid ID format", nameof(id));
            }

            await _usersCollection.ReplaceOneAsync(user => user.Id == objectId.ToString(), userIn);
        }

        public async Task DeleteUserAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new ArgumentException("Invalid ID format", nameof(id));
            }

            await _usersCollection.DeleteOneAsync(user => user.Id == objectId.ToString());
        }
        public async Task AddPatientToUserAsync(string userId, string patientId, string relationship)
        {
            if (!ObjectId.TryParse(userId, out var userObjectId) || !ObjectId.TryParse(patientId, out var patientObjectId))
            {
                throw new ArgumentException("Invalid ID format");
            }

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Push(u => u.Patients, new Patient { PatientId = patientId, Relationship = relationship });

            await _usersCollection.UpdateOneAsync(filter, update);
        }

    }
}
