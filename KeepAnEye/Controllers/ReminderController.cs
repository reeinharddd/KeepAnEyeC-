using Microsoft.AspNetCore.Mvc;
using KeepAnEye.Models;
using KeepAnEye.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderService _reminderService;

        public ReminderController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRemindersByUserId(string userId)
        {
            var reminders = await _reminderService.GetRemindersByUserIdAsync(userId);
            if (reminders == null || reminders.Count == 0)
            {
                return NotFound("No reminders found for the specified user.");
            }
            return Ok(reminders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReminderById(string id)
        {
            var reminder = await _reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }
            return Ok(reminder);
        }

        [HttpPost]
        public async Task<ActionResult<Reminder>> CreateReminder(Reminder reminder)
        {
            await _reminderService.CreateReminderAsync(reminder);
            return CreatedAtAction(nameof(GetReminderById), new { id = reminder.Id }, reminder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(string id, Reminder reminderIn)
        {
            var reminder = await _reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }
            await _reminderService.UpdateReminderAsync(id, reminderIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(string id)
        {
            var reminder = await _reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }
            await _reminderService.DeleteReminderAsync(id);
            return NoContent();
        }
    }

}
