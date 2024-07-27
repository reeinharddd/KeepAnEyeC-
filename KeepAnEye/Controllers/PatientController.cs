using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly UserService _userService;

        public PatientController(UserService userService)
        {
            _userService = userService;
        }

        // POST api/patient
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] User patient)
        {
            if (string.IsNullOrEmpty(patient.Name.FirstName) || string.IsNullOrEmpty(patient.Name.LastName) ||
                string.IsNullOrEmpty(patient.Email) || string.IsNullOrEmpty(patient.Password))
            {
                return BadRequest("First Name, Last Name, Email, and Password are required.");
            }

            patient.UserType = "patient"; // Aseguramos que el tipo de usuario sea 'patient'
            patient.Subscription = null;  // No asignamos ningún valor a Subscription
            patient.Patients = null;      // No asignamos ningún valor a Patients

            await _userService.CreateUserAsync(patient);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, new { id = patient.Id });
        }

        // GET api/patient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetPatient(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var patient = await _userService.GetUserAsync(id);
            if (patient == null || patient.UserType != "patient")
            {
                return NotFound();
            }
            return Ok(patient);
        }
    }
}
