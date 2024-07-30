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

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] User patient)
        {
            if (string.IsNullOrEmpty(patient.Name.FirstName) || string.IsNullOrEmpty(patient.Name.LastName) ||
                string.IsNullOrEmpty(patient.Email) || string.IsNullOrEmpty(patient.Password))
            {
                return BadRequest("First Name, Last Name, Email, and Password are required.");
            }

            patient.UserType = "patient";
            patient.Subscription = null;
            patient.Patients = null;

            var existingPatient = (await _userService.GetUsersAsync()).FirstOrDefault(u => u.Email == patient.Email);
            if (existingPatient != null)
            {
                return BadRequest("Email is already in use.");
            }

            await _userService.CreateUserAsync(patient);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, new { id = patient.Id });
        }

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
