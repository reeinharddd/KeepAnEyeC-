// Services/UserService.cs
using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;

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
    }
}
