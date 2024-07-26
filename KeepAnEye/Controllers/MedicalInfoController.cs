// Controllers/MedicalInfoController.cs
using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalInfoController : ControllerBase
    {
        private readonly MedicalInfoService _medicalInfoService;

        public MedicalInfoController(MedicalInfoService medicalInfoService)
        {
            _medicalInfoService = medicalInfoService;
        }

        [HttpGet("{patientId}")]
        [Authorize]
        public async Task<IActionResult> GetMedicalInfoByPatientId(string patientId)
        {
            var medicalInfo = await _medicalInfoService.GetMedicalInfoByPatientIdAsync(patientId);
            if (medicalInfo == null)
            {
                return NotFound(new { message = "Medical info not found" });
            }
            return Ok(medicalInfo);
        }

        [HttpGet("documents/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetDocuments(string userId)
        {
            var documents = await _medicalInfoService.GetDocumentsAsync(userId);
            if (documents == null || !documents.Any())
            {
                return NotFound(new { message = "Medical info not found for the user" });
            }
            return Ok(documents);
        }
    }
}
