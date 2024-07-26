using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KeepAnEye.Models
{
    public class Metric
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("patient_id")]
        public ObjectId PatientId { get; set; }

        [BsonElement("heartRate")]
        public List<MetricEntry> HeartRate { get; set; }

        [BsonElement("temperature")]
        public List<MetricEntry> Temperature { get; set; }

        [BsonElement("location")]
        public List<LocationEntry> Location { get; set; }
    }

    public class MetricEntry
    {
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("value")]
        public double Value { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }
    }

    public class LocationEntry
    {
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("coordinates")]
        public Coordinates Coordinates { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }
    }

    public class Coordinates
    {
        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }
    }
}
