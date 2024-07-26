using KeepAnEye.Models.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KeepAnEye.Models
{
    [BsonIgnoreExtraElements]
    public class MedicalInfo 
    {
        public ObjectId Id { get; set; }

        [BsonElement("patient_id")]
        public ObjectId PatientId { get; set; }

        [BsonElement("health_info")]
        public HealthInfo HealthInfo { get; set; }

        [BsonElement("NSS")]
        public string NSS { get; set; }

        [BsonElement("blood_type")]
        public string BloodType { get; set; }

        [BsonElement("height")]
        public string Height { get; set; }

        [BsonElement("weight")]
        public string Weight { get; set; }

        [BsonElement("hospitals")]
        public List<Hospital> Hospitals { get; set; }

        [BsonElement("medical_documents")]
        public List<MedicalDocument> MedicalDocuments { get; set; }
    }

    public class HealthInfo
    {
        [BsonElement("medicines")]
        public List<Medicine> Medicines { get; set; }

        [BsonElement("allergies")]
        public List<Allergy> Allergies { get; set; }

        [BsonElement("medical_conditions")]
        public List<MedicalCondition> MedicalConditions { get; set; }
    }

    public class Medicine
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("dosage")]
        public string Dosage { get; set; }

        [BsonElement("frequency")]
        public string Frequency { get; set; }
    }

    public class Allergy
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("severity")]
        public string Severity { get; set; }

        [BsonElement("reaction")]
        public string Reaction { get; set; }

        [BsonElement("treatment")]
        public string Treatment { get; set; }
    }

    public class MedicalCondition
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("severity")]
        public string Severity { get; set; }

        [BsonElement("treatment")]
        public string Treatment { get; set; }

        [BsonElement("symptoms")]
        public string Symptoms { get; set; }

        [BsonElement("diagnosis_date")]
        public DateTime DiagnosisDate { get; set; }
    }

    public class Hospital
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
    }

    public class MedicalDocument
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }
    }
}
