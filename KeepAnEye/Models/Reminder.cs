using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace KeepAnEye.Models
{
    public class Reminder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userToRemind")]
        public string UserToRemind { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }
}
