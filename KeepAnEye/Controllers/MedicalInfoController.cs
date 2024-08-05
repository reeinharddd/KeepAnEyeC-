using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost("medicalInfo")]
        public async Task<IActionResult> CreateMedicalInfo([FromBody] MedicalInfo medicalInfo)
        {
            await _medicalInfoService.CreateMedicalInfoAsync(medicalInfo);
            return CreatedAtAction(nameof(GetMedicalInfoByPatientId), new { patientId = medicalInfo.PatientId.ToString() }, medicalInfo);
        }

        [HttpPut("{patientId}")]
        public async Task<IActionResult> UpdateMedicalInfo(string patientId, [FromBody] MedicalInfo updatedInfo)
        {

            var result = await _medicalInfoService.UpdateMedicalInfoAsync(patientId, updatedInfo);
            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Medical info not found" });
            }

            return Ok(new { message = "Medical info updated successfully" });
        }
    }
}
