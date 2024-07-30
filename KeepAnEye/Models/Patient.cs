using KeepAnEye.Models.Abstract;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace KeepAnEye.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public Name Name { get; set; } = new Name();

        [BsonElement("sex")]
        public string Sex { get; set; } = null;

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
        public string Phone { get; set; } = null;

        [BsonElement("address")]
        public Address Address { get; set; } = new Address();

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("status")]
        public string Status { get; set; } = null;
    }

}
