using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace KeepAnEye.Models
{
    [BsonIgnoreExtraElements]
    public class EmergencieContacts
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("patient_id")]
        public ObjectId PatientId { get; set; }

        [BsonElement("emergency_contacts")]
        public List<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

    }
    public class ContactInformation
    {
        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
    }

    public class Notification
    {
        [BsonElement("type")]
        public string Type { get; set; } = "default_type_value";

        [BsonElement("status")]
        public string Status { get; set; } = "default_status_value";
    }

    public class EmergencyContact
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("relationship")]
        public string Relationship { get; set; } = string.Empty;

        [BsonElement("contactInformation")]
        public ContactInformation ContactInformation { get; set; } = new ContactInformation();

        [BsonElement("notifications")]
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
