// Services/MedicalInfoService.cs
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KeepAnEye.Services
{
    public class MedicalInfoService
    {
        private readonly IMongoCollection<MedicalInfo> _medicalInfos;

        public MedicalInfoService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _medicalInfos = mongoDatabase.GetCollection<MedicalInfo>("medicalInfo");
        }

        public async Task<MedicalInfo?> GetMedicalInfoByPatientIdAsync(string patientId)
        {


            return await _medicalInfos.Find(medicalInfo => medicalInfo.PatientId == patientId).FirstOrDefaultAsync();
        }


        public async Task<List<MedicalDocument>> GetDocumentsAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
            {
                return new List<MedicalDocument>(); // O maneja el error según tu lógica de negocio
            }

            var medicalInfo = await _medicalInfos.Find(medicalInfo => medicalInfo.PatientId == userId).FirstOrDefaultAsync();
            if (medicalInfo != null)
            {
                return medicalInfo.MedicalDocuments.OrderByDescending(doc => doc.Date).ToList();
            }
            return new List<MedicalDocument>();
        }

        // Método para crear nueva información médica
        public async Task CreateMedicalInfoAsync(MedicalInfo medicalInfo)
        {
            await _medicalInfos.InsertOneAsync(medicalInfo);
        }

        public async Task<UpdateResult> UpdateMedicalInfoAsync(string patientId, MedicalInfo updatedInfo)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update
                .Set(info => info.HealthInfo, updatedInfo.HealthInfo)
                .Set(info => info.Height, updatedInfo.Height)
                .Set(info => info.Weight, updatedInfo.Weight)
                .Set(info => info.Hospitals, updatedInfo.Hospitals)
                .Set(info => info.MedicalDocuments, updatedInfo.MedicalDocuments);

            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

    }
}