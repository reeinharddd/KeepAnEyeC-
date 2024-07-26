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
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<EmergencieContacts> _emergencieContactsCollection;
        private readonly IMongoCollection<MedicalInfo> _medicalInfoCollection;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _usersCollection = database.GetCollection<User>("users");
            _emergencieContactsCollection = database.GetCollection<EmergencieContacts>("emergencieContacts");
            _medicalInfoCollection = database.GetCollection<MedicalInfo>("medicalInfo");

        }


        public IMongoCollection<User> GetUsersCollection()
        {
            return _usersCollection;
        }


        public void CreateUser(User user)
        {
            _usersCollection.InsertOne(user); // MongoDB generará automáticamente el ID
        }

        public List<User> GetUsers()
        {
            return _usersCollection.Find(user => true).ToList();
        }

        public User GetUser(string id)
        {
            return _usersCollection.Find(user => user.Id == id).FirstOrDefault();
        }

        public void UpdateUser(string id, UpdateDefinition<User> updateDefinition)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            _usersCollection.UpdateOne(filter, updateDefinition);
        }

        public void DeleteUser(string id)
        {
            _usersCollection.DeleteOne(user => user.Id == id);
        }

        // Métodos para PatientEmergencyContacts
        public void CreatePatientEmergencyContacts(EmergencieContacts emergencyContact)
        {
            _emergencieContactsCollection.InsertOne(emergencyContact);
        }

        public EmergencieContacts GetPatientEmergencyContacts(ObjectId patientId)
        {
            return _emergencieContactsCollection
                .Find(emergencyContact => emergencyContact.PatientId == patientId)
                .FirstOrDefault();
        }

        public void UpdateEmergencieContactsByPatientId(ObjectId patientId, List<EmergencyContact> updatedEmergencyContacts)
        {
            var filter = Builders<EmergencieContacts>.Filter.Eq(ec => ec.PatientId, patientId);
            var updateDefinition = Builders<EmergencieContacts>.Update
                .Set(ec => ec.EmergencyContacts, updatedEmergencyContacts);

            var result = _emergencieContactsCollection.UpdateOne(filter, updateDefinition);

            if (result.ModifiedCount == 0)
            {
                throw new InvalidOperationException("No documents matched the filter criteria.");
            }
        }

        // Métodos para MedicalInfo
        public void CreateMedicalInfo(MedicalInfo medicalInfo)
        {
            _medicalInfoCollection.InsertOne(medicalInfo);
        }

        public MedicalInfo GetMedicalInfoByPatientId(ObjectId patientId)
        {
            return _medicalInfoCollection
                .Find(mi => mi.PatientId == patientId) // Consulta usando ObjectId
                .FirstOrDefault();
        }


        public void UpdateMedicalInfoByPatientId(ObjectId patientId, MedicalInfo updatedMedicalInfo)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(mi => mi.PatientId, patientId);
            var updateDefinition = Builders<MedicalInfo>.Update
                .Set(mi => mi.HealthInfo, updatedMedicalInfo.HealthInfo)
                .Set(mi => mi.Hospitals, updatedMedicalInfo.Hospitals)
                .Set(mi => mi.MedicalDocuments, updatedMedicalInfo.MedicalDocuments);

            var result = _medicalInfoCollection.UpdateOne(filter, updateDefinition);

            if (result.ModifiedCount == 0)
            {
                throw new InvalidOperationException("No documents matched the filter criteria.");
            }
        }


    }

    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}
