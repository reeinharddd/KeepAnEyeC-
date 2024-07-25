using Microsoft.AspNetCore.Mvc;
using KeepAnEye.Models;
using KeepAnEye.Services;
using System.Threading.Tasks;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsService _metricsService;

        public MetricsController(MetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet("location/{patientId}")]
        public async Task<IActionResult> GetLocation(string patientId)
        {
            var metric = await _metricsService.GetLocationAsync(patientId);
            if (metric == null)
            {
                return NotFound("Metric not found");
            }
            return Ok(metric.Location);
        }

        [HttpPost("location/{patientId}")]
        public async Task<IActionResult> UpdateLocation(string patientId, [FromBody] LocationEntry locationEntry)
        {
            var metric = await _metricsService.UpdateLocationAsync(patientId, locationEntry);
            if (metric == null)
            {
                return NotFound("Metric not found");
            }

            // Aquí puedes emitir el evento a través de SignalR si deseas enviar notificaciones en tiempo real
            // Ejemplo: _hubContext.Clients.All.SendAsync("locationUpdate", metric.Location);

            return Ok(metric.Location);
        }
        [HttpGet("all/{patientId}")]
        public async Task<IActionResult> GetAllMetricsByPatientId(string patientId)
        {
            var metrics = await _metricsService.GetMetricsByPatientIdAsync(patientId);
            if (metrics == null || metrics.Count == 0)
            {
                return NotFound("No metrics found for the specified patient.");
            }
            return Ok(metrics);
        }
    }
}
