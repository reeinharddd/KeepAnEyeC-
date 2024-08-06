using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<MedicalInfo> EnsureMedicalInfoExistsAsync(string patientId)
        {
            var medicalInfo = await _medicalInfos.Find(info => info.PatientId == patientId).FirstOrDefaultAsync();
            if (medicalInfo == null)
            {
                medicalInfo = new MedicalInfo
                {
                    PatientId = patientId,
                    HealthInfo = new HealthInfo(),
                    NSS = string.Empty,
                    BloodType = string.Empty,
                    Height = string.Empty,
                    Weight = string.Empty,
                    Hospitals = new List<Hospital>(),
                    MedicalDocuments = new List<MedicalDocument>()
                };
                await _medicalInfos.InsertOneAsync(medicalInfo);
            }
            return medicalInfo;
        }

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
        public async Task<UpdateResult> UpdateMedicalInfoFieldAsync(FilterDefinition<MedicalInfo> filter, UpdateDefinition<MedicalInfo> update)
        {
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }
        public async Task<UpdateResult> UpdateMedicinesAsync(string patientId, List<Medicine> newMedicines)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);

            // Generar nuevos Ids si no se proporcionan
            foreach (var medicine in newMedicines)
            {
                if (string.IsNullOrEmpty(medicine.Id))
                {
                    medicine.Id = ObjectId.GenerateNewId().ToString(); // Genera un nuevo Id para el medicamento
                }
            }

            var update = Builders<MedicalInfo>.Update.PushEach(info => info.HealthInfo.Medicines, newMedicines);

            return await _medicalInfos.UpdateOneAsync(filter, update);
        }
        public async Task<UpdateResult> UpdateAllergiesAsync(string patientId, List<Allergy> newAllergies)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.AddToSetEach(info => info.HealthInfo.Allergies, newAllergies);
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }


        public async Task<UpdateResult> UpdateMedicalConditionsAsync(string patientId, List<MedicalCondition> newConditions)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.AddToSetEach(info => info.HealthInfo.MedicalConditions, newConditions);
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }
        public async Task<UpdateResult> UpdateHospitalsAsync(string patientId, List<Hospital> newHospitals)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.AddToSetEach(info => info.Hospitals, newHospitals);
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }
        public async Task<UpdateResult> UpdateMedicalDocumentsAsync(string patientId, List<MedicalDocument> newDocuments)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.AddToSetEach(info => info.MedicalDocuments, newDocuments);
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateMedicalInfoFieldsAsync(string patientId, string? nss, string? bloodType, string? height, string? weight)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);

            var updateDefinition = Builders<MedicalInfo>.Update
                .Set(info => info.NSS, nss)
                .Set(info => info.BloodType, bloodType)
                .Set(info => info.Height, height)
                .Set(info => info.Weight, weight);

            return await _medicalInfos.UpdateOneAsync(filter, updateDefinition);
        }

        public async Task<UpdateResult> RemoveAllergyAsync(string patientId, int index)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.PullFilter(info => info.HealthInfo.Allergies, Builders<Allergy>.Filter.Eq("Index", index));
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveMedicalConditionAsync(string patientId, int index)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.PullFilter(info => info.HealthInfo.MedicalConditions, Builders<MedicalCondition>.Filter.Eq("Index", index));
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveHospitalAsync(string patientId, int index)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.PullFilter(
                info => info.Hospitals,
                Builders<Hospital>.Filter.Eq("Id", index.ToString()) // Ajusta esto si tu lógica de índice es diferente
            );
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveMedicalDocumentAsync(string patientId, int index)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.PullFilter(info => info.MedicalDocuments, Builders<MedicalDocument>.Filter.Eq("Index", index));
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }
        public async Task<UpdateResult> RemoveMedicineAsync(string patientId, string medicineId)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);
            var update = Builders<MedicalInfo>.Update.PullFilter(info => info.HealthInfo.Medicines, Builders<Medicine>.Filter.Eq(m => m.Id, medicineId));
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveMedicineByIndexAsync(string patientId, int index)
        {
            var filter = Builders<MedicalInfo>.Filter.Eq(info => info.PatientId, patientId);

            // Assuming positions are relevant and are maintained as indices
            var update = Builders<MedicalInfo>.Update.PullFilter(info => info.HealthInfo.Medicines, Builders<Medicine>.Filter.Eq("Index", index));
            return await _medicalInfos.UpdateOneAsync(filter, update);
        }

    }
}
