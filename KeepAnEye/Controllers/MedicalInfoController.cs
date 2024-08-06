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
                return NotFound("MedicalInfo not found");
            }
            return NoContent();
        }
        [HttpPost("{patientId}")]
        public async Task<IActionResult> EnsureMedicalInfoExists(string patientId)
        {
            var medicalInfo = await _medicalInfoService.EnsureMedicalInfoExistsAsync(patientId);
            return Ok(medicalInfo);
        }
        [HttpPost("{patientId}/medicines")]
        [Authorize]
        public async Task<IActionResult> UpdateMedicines(string patientId, MedicinesUpdateRequest request)
        {
            var medicalInfo = await _medicalInfoService.GetMedicalInfoByPatientIdAsync(patientId);
            if (medicalInfo == null)
            {
                return NotFound(new { message = "Medical info not found" });
            }

            var result = await _medicalInfoService.UpdateMedicinesAsync(patientId, request.NewMedicines);
            if (result.ModifiedCount > 0)
            {
                return Ok(new { message = "Medicines updated successfully" });
            }

            return BadRequest(new { message = "No changes were made" });
        }

        [HttpPatch("{patientId}/allergies")]
        public async Task<IActionResult> UpdateAllergies(string patientId, [FromBody] List<Allergy> allergies)
        {
            var result = await _medicalInfoService.UpdateAllergiesAsync(patientId, allergies);
            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Medical info not found" });
            }
            return NoContent();
        }

        [HttpPatch("{patientId}/conditions")]
        public async Task<IActionResult> UpdateMedicalConditions(string patientId, [FromBody] List<MedicalCondition> conditions)
        {
            var result = await _medicalInfoService.UpdateMedicalConditionsAsync(patientId, conditions);
            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Medical info not found" });
            }
            return NoContent();
        }


        [HttpPatch("{patientId}/hospitals")]
        public async Task<IActionResult> UpdateHospitals(string patientId, [FromBody] List<Hospital> hospitals)
        {
            var result = await _medicalInfoService.UpdateHospitalsAsync(patientId, hospitals);
            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Medical info not found" });
            }
            return NoContent();
        }

        [HttpPatch("{patientId}/documents")]
        public async Task<IActionResult> UpdateMedicalDocuments(string patientId, [FromBody] List<MedicalDocument> documents)
        {
            var result = await _medicalInfoService.UpdateMedicalDocumentsAsync(patientId, documents);
            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Medical info not found" });
            }
            return NoContent();
        }

        [HttpPatch("{patientId}/update-fields")]
        public async Task<IActionResult> UpdateMedicalInfoFields(string patientId, [FromBody] MedicalInfoUpdateRequest updateRequest)
        {
            var result = await _medicalInfoService.UpdateMedicalInfoFieldsAsync(patientId, updateRequest.NSS, updateRequest.BloodType, updateRequest.Height, updateRequest.Weight);
            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Medical info not found" });
            }
            return NoContent();
        }
        [HttpDelete("{patientId}/allergies/{index}")]
        [Authorize]
        public async Task<IActionResult> DeleteAllergy(string patientId, int index)
        {
            var medicalInfo = await _medicalInfoService.GetMedicalInfoByPatientIdAsync(patientId);
            if (medicalInfo == null || medicalInfo.HealthInfo == null || medicalInfo.HealthInfo.Allergies == null || index < 0 || index >= medicalInfo.HealthInfo.Allergies.Count)
            {
                return NotFound();
            }

            medicalInfo.HealthInfo.Allergies.RemoveAt(index);
            var result = await _medicalInfoService.UpdateMedicalInfoAsync(patientId, medicalInfo);

            if (result.ModifiedCount == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the database");
            }

            return NoContent();
        }
        [HttpDelete("{patientId}/medicines/{medicineId}")]
        public async Task<IActionResult> DeleteMedicine(string patientId, string medicineId)
        {
            var result = await _medicalInfoService.RemoveMedicineAsync(patientId, medicineId);
            if (result.ModifiedCount > 0)
            {
                return NoContent();
            }
            return NotFound();
        }





        [HttpDelete("{patientId}/conditions/{index}")]
        [Authorize]
        public async Task<IActionResult> DeleteCondition(string patientId, int index)
        {
            var result = await _medicalInfoService.RemoveMedicalConditionAsync(patientId, index);
            if (result.ModifiedCount == 0)
            {
                return NotFound(new { message = "Medical condition not found or index is invalid" });
            }
            return NoContent();
        }

        [HttpDelete("{patientId}/hospitals/{index}")]
        public async Task<IActionResult> DeleteHospital(string patientId, int index)
        {
            var result = await _medicalInfoService.RemoveHospitalAsync(patientId, index);
            if (result.ModifiedCount == 0)
            {
                return NotFound(new { message = "Hospital not found" });
            }
            return NoContent(); // 204 No Content
        }



        [HttpDelete("{patientId}/documents/{index}")]
        [Authorize]
        public async Task<IActionResult> DeleteMedicalDocument(string patientId, int index)
        {
            var result = await _medicalInfoService.RemoveMedicalDocumentAsync(patientId, index);
            if (result.ModifiedCount == 0)
            {
                return NotFound(new { message = "Medical document not found or index is invalid" });
            }
            return NoContent();
        }



    }
}
