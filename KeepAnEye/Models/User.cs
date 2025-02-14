using KeepAnEye.Models.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KeepAnEye.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public Name Name { get; set; } = new Name();

        [BsonElement("sex")]
        public string? Sex { get; set; }

        [BsonElement("user_photo")]
        public string? UserPhoto { get; set; }

        [BsonElement("born_date")]
        public DateTime BornDate { get; set; } = DateTime.UtcNow;

        [BsonElement("user_type")]
        public string UserType { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; } = new Address();

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("status")]
        public string? Status { get; set; }

        [BsonElement("subscription")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Subscription { get; set; }


        [BsonElement("patients")]
        public List<Patient> Patients { get; set; } = new List<Patient>();
    }

    public class Name
    {
        [BsonElement("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("last_name")]
        public string LastName { get; set; } = string.Empty;
    }

    public class Patient
    {
        [BsonElement("patient_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }

        [BsonElement("relationship")]
        public string Relationship { get; set; } = null;

        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

}
