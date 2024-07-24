﻿using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using MongoDB.Bson;

namespace KeepAnEye.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _usersCollection = database.GetCollection<User>("users");
        }

        public void CreateUser(User user)
        {
            _usersCollection.InsertOne(user);
        }

        public List<User> GetUsers()
        {
            return _usersCollection.Find(user => true).ToList();
        }

        public User GetUser(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null;
            }

            return _usersCollection.Find(user => user.Id == objectId.ToString()).FirstOrDefault();
        }

        public void UpdateUser(string id, User userIn)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new ArgumentException("Invalid ID format", nameof(id));
            }

            _usersCollection.ReplaceOne(user => user.Id == objectId.ToString(), userIn);
        }

        public void DeleteUser(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new ArgumentException("Invalid ID format", nameof(id));
            }

            _usersCollection.DeleteOne(user => user.Id == objectId.ToString());
        }
    }

    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}
