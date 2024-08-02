using MongoDB.Driver;
using KeepAnEye.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace KeepAnEye.Services
{
    public class AppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var settings = mongoDbSettings.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _appointments = database.GetCollection<Appointment>("appointments");
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(string patientId)
        {
            var filter = Builders<Appointment>.Filter.Eq("PatientId", patientId); // No se convierte a ObjectId
            return await _appointments.Find(filter).ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            var filter = Builders<Appointment>.Filter.Eq("Id", id); // No se convierte a ObjectId
            return await _appointments.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            await _appointments.InsertOneAsync(appointment);
        }

        public async Task UpdateAppointmentAsync(string id, Appointment appointmentIn)
        {
            var filter = Builders<Appointment>.Filter.Eq("Id", id); // No se convierte a ObjectId
            await _appointments.ReplaceOneAsync(filter, appointmentIn);
        }

        public async Task DeleteAppointmentAsync(string id)
        {
            var filter = Builders<Appointment>.Filter.Eq("Id", id); // No se convierte a ObjectId
            await _appointments.DeleteOneAsync(filter);
        }
    }
}
