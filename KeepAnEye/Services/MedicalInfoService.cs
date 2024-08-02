using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeepAnEye.Services
{
    public class MedicalInfoService
    {
        private readonly IMongoCollection<MedicalInfo> _medicalInfos;
        private readonly FirebaseService _firebaseService;

        public MedicalInfoService(IOptions<MongoDbSettings> mongoDbSettings, FirebaseService firebaseService)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _medicalInfos = mongoDatabase.GetCollection<MedicalInfo>("medicalInfo");
            _firebaseService = firebaseService;
        }

        public async Task<MedicalInfo?> GetMedicalInfoByPatientIdAsync(string patientId)
        {
            return await _medicalInfos.Find(medicalInfo => medicalInfo.PatientId == patientId).FirstOrDefaultAsync();
        }

        public async Task<List<MedicalDocument>> GetDocumentsAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
            {
                return new List<MedicalDocument>();
            }

            var medicalInfo = await _medicalInfos.Find(medicalInfo => medicalInfo.PatientId == userId).FirstOrDefaultAsync();
            if (medicalInfo != null)
            {
                return medicalInfo.MedicalDocuments.OrderByDescending(doc => doc.Date).ToList();
            }
            return new List<MedicalDocument>();
        }

        public async Task CreateMedicalInfoAsync(MedicalInfo medicalInfo)
        {
            if (medicalInfo.MedicalDocuments != null)
            {
                foreach (var doc in medicalInfo.MedicalDocuments)
                {
                    // Asegúrate de proporcionar un Stream de archivo real aquí
                    var fileStream = new MemoryStream(); // Reemplaza con el flujo de archivo real
                    doc.Url = await _firebaseService.UploadFileToFirebaseAsync(fileStream, doc.Name);
                }
            }

            await _medicalInfos.InsertOneAsync(medicalInfo);
        }

        public async Task<UpdateResult> UpdateMedicalInfoAsync(string patientId, MedicalInfo updatedInfo)
        {
            if (updatedInfo.MedicalDocuments != null)
            {
                foreach (var doc in updatedInfo.MedicalDocuments)
                {
                    // Asegúrate de proporcionar un Stream de archivo real aquí
                    var fileStream = new MemoryStream(); // Reemplaza con el flujo de archivo real
                    doc.Url = await _firebaseService.UploadFileToFirebaseAsync(fileStream, doc.Name);
                }
            }

            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update
                .Set(info => info.HealthInfo, updatedInfo.HealthInfo)
                .Set(info => info.Height, updatedInfo.Height)
                .Set(info => info.Weight, updatedInfo.Weight)
                .Set(info => info.Hospitals, updatedInfo.Hospitals)
                .Set(info => info.MedicalDocuments, updatedInfo.MedicalDocuments);

            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

        internal async Task<string> UploadFileToFirebaseAsync(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
