using MongoDB.Bson.Serialization.Attributes;

namespace KeepAnEye.Models.Abstract
{
    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("zip")]
        public string Zip { get; set; } = string.Empty;
    }
}
