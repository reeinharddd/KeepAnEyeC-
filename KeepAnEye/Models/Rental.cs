using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace KeepAnEye.Models
{
    [BsonIgnoreExtraElements]
    public class Rental
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id"), BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        public DateTime EndDate { get; set; }

        [BsonElement("rental_price")]
        public double RentalPrice { get; set; }
       
        [BsonElement(" payment_id"), BsonRepresentation(BsonType.ObjectId)]
        public string PaymentId { get; set; }

        [BsonElement("payment_date")]
        public DateTime PaymentDate {  get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = null;
    }
}
