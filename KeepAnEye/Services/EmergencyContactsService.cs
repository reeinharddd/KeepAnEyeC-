using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KeepAnEye.Services
{
    public class EmergencyContactsService
    {
        private readonly IMongoCollection<EmergencieContacts> _emergencyContacts;

        public EmergencyContactsService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _emergencyContacts = mongoDatabase.GetCollection<EmergencieContacts>("emergencieContacts");
        }

        // Método para crear nuevos contactos de emergencia
        public async Task CreateEmergencyContactsAsync(EmergencieContacts emergencyContacts)
        {
            await _emergencyContacts.InsertOneAsync(emergencyContacts);
        }

        // Método para obtener contactos de emergencia por ID de paciente
        public async Task<EmergencieContacts?> GetEmergencyContactsByPatientIdAsync(string patientId)
        {

            return await _emergencyContacts.Find(ec => ec.PatientId == patientId).FirstOrDefaultAsync();
        }

        // Método para actualizar contactos de emergencia por ID de paciente
        public async Task<bool> UpdateEmergencyContactsByPatientIdAsync(string patientId, List<EmergencyContact> updatedEmergencyContacts)
        {
         

            var filter = Builders<EmergencieContacts>.Filter.Eq(ec => ec.PatientId, patientId);
            var updateDefinition = Builders<EmergencieContacts>.Update
                .Set(ec => ec.EmergencyContacts, updatedEmergencyContacts);

            var result = await _emergencyContacts.UpdateOneAsync(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }
    }
}
