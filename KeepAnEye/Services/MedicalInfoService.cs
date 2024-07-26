// Services/MedicalInfoService.cs
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
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
            var medicalInfo = await _medicalInfos.Find(medicalInfo => medicalInfo.PatientId == userId).FirstOrDefaultAsync();
            if (medicalInfo != null)
            {
                return medicalInfo.MedicalDocuments.OrderByDescending(doc => doc.Date).ToList();
            }
            return new List<MedicalDocument>();
        }
    }
}
