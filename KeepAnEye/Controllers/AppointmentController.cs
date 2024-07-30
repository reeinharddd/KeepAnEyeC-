using Microsoft.AspNetCore.Mvc;
using KeepAnEye.Models;
using KeepAnEye.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatientId(string patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("No appointments found for the specified patient.");
            }
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(string id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            await _appointmentService.CreateAppointmentAsync(appointment);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(string id, Appointment appointmentIn)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            await _appointmentService.UpdateAppointmentAsync(id, appointmentIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(string id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            await _appointmentService.DeleteAppointmentAsync(id);
            return NoContent();
        }
    }
}
