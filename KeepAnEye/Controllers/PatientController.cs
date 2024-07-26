using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly MongoDbService _mongoDbService;

        public PatientController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // POST api/patient
        [HttpPost]
        public IActionResult CreatePatient([FromBody] User patient)
        {
            if (string.IsNullOrEmpty(patient.Name.FirstName) || string.IsNullOrEmpty(patient.Name.LastName) ||
                string.IsNullOrEmpty(patient.Email) || string.IsNullOrEmpty(patient.Password))
            {
                return BadRequest("First Name, Last Name, Email, and Password are required.");
            }

            patient.UserType = "patient"; // Aseguramos que el tipo de usuario sea 'patient'
            patient.Subscription = null;  // No asignamos ningún valor a Subscription
            patient.Patients = null;      // No asignamos ningún valor a Patients

            _mongoDbService.CreateUser(patient);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, new { id = patient.Id });
        }

        // GET api/patient/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetPatient(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var patient = _mongoDbService.GetUser(id);
            if (patient == null || patient.UserType != "patient")
            {
                return NotFound();
            }
            return Ok(patient);
        }

        // POST api/patient/{patientId}/emergencyContact
        [HttpPost("{patientId}/emergencyContact")]
        public IActionResult CreateEmergencyContact(string patientId, [FromBody] EmergencieContacts emergencyContact)
        {
            try
            {
                if (!ObjectId.TryParse(patientId, out var objectId))
                {
                    return BadRequest("Invalid patient ID format.");
                }

                emergencyContact.PatientId = objectId; // Asignar el ID del paciente a EmergencieContacts

                _mongoDbService.CreatePatientEmergencyContacts(emergencyContact);
                return CreatedAtAction(nameof(GetPatientEmergencyContacts), new { patientId = emergencyContact.PatientId }, emergencyContact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET api/patient/{patientId}/emergency-contacts
        [HttpGet("emergency-contacts/{patientId}")]
        public ActionResult<EmergencieContacts> GetPatientEmergencyContacts(string patientId)
        {
            try
            {
                // Convertir el patientId a ObjectId
                if (!ObjectId.TryParse(patientId, out var objectId))
                {
                    return BadRequest("Invalid patient ID format.");
                }

                var emergencyContacts = _mongoDbService.GetPatientEmergencyContacts(objectId);
                if (emergencyContacts == null)
                {
                    return NotFound();
                }
                return Ok(emergencyContacts);
            }
            catch (Exception ex)
            {
                // Puedes usar un tipo de excepción más específico si es posible
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/patient/{patientId}/medicalInfo
        [HttpPost("{patientId}/medicalInfo")]
        public IActionResult CreateMedicalInfo(string patientId, [FromBody] MedicalInfo medicalInfo)
        {
            try
            {
                if (!ObjectId.TryParse(patientId, out var objectId))
                {
                    return BadRequest("Invalid patient ID format.");
                }

                medicalInfo.PatientId = patientId; // Asignar el ID del paciente a MedicalInfo

                _mongoDbService.CreateMedicalInfo(medicalInfo);
                return CreatedAtAction(nameof(GetMedicalInfo), new { patientId = medicalInfo.PatientId }, medicalInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET api/Patient/{patientId}/medicalInfo
        [HttpGet("medicalInfo/{patientId}")]
        public ActionResult<MedicalInfo> GetMedicalInfo(string patientId)
        {
            try
            {
                // Convertir el patientId a ObjectId
                if (!ObjectId.TryParse(patientId, out var objectId))
                {
                    return BadRequest("Invalid patient ID format.");
                }

                var medicalInfo = _mongoDbService.GetMedicalInfoByPatientId(patientId);
                if (medicalInfo == null)
                {
                    return NotFound();
                }
                return Ok(medicalInfo);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                return StatusCode(500, ex.Message);
            }
        }

    }
}
