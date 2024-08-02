using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KeepAnEye.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("patient_id")]
        public string PatientId { get; set; }

        [BsonElement("appointments")]
        public List<AppointmentDetail> Appointments { get; set; }
    }

    public class AppointmentDetail
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("time")]
        public TimeSpan Time { get; set; }

        [BsonElement("place")]
        public string Place { get; set; }
        
        [BsonElement("status")]
                public string Status { get; set; }

    }
}
