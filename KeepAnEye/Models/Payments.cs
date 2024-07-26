using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KeepAnEye.Models
{
    [BsonIgnoreExtraElements]
    public class Payments
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("card_name")]
        public string CardName { get; set; } = string.Empty;

        [BsonElement("card_number")]
        public string CardNumber { get; set; } = string.Empty;

        [BsonElement("expiration_date")]
        public string ExpirationDate { get; set; } = string.Empty;

        [BsonElement("CVV")]
        public string CVV { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; }
    }
}
