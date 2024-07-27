using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("emercencieContacts/[controller]")]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly EmergencyContactsService _emergencyContactsService;

        public EmergencyContactsController(EmergencyContactsService emergencyContactsService)
        {
            _emergencyContactsService = emergencyContactsService;

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEmergencyContacts([FromBody] EmergencieContacts emergencyContacts)
        {
            await _emergencyContactsService.CreateEmergencyContactsAsync(emergencyContacts);
            return CreatedAtAction(nameof(GetEmergencyContactsByPatientId), new { patientId = emergencyContacts.PatientId.ToString() }, emergencyContacts);
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetEmergencyContactsByPatientId(string patientId)
        {
            var emergencyContacts = await _emergencyContactsService.GetEmergencyContactsByPatientIdAsync(patientId);
            if (emergencyContacts == null)
            {
                return NotFound(new { message = "Emergency contacts not found" });
            }
            return Ok(emergencyContacts);
        }

        [HttpPut("{patientId}")]
        public async Task<IActionResult> UpdateEmergencyContactsByPatientId(string patientId, [FromBody] List<EmergencyContact> updatedEmergencyContacts)
        {
            var success = await _emergencyContactsService.UpdateEmergencyContactsByPatientIdAsync(patientId, updatedEmergencyContacts);
            if (!success)
            {
                return NotFound(new { message = "Emergency contacts not found or failed to update" });
            }
            return NoContent();
        }
    }
}
