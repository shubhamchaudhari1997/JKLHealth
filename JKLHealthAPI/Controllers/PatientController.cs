using JKLHealthAPI.Data;
using JKLHealthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetPatients")]
        public async Task<IActionResult> GetPatients()
        {
            try
            {
                var patients = await _context.Patient.ToListAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching patients", Details = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient(Patient patient)
        {
            if (patient == null)
            {
                return BadRequest("Patient data cannot be null.");
            }

            _context.Patient.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        // GET: api/Patient/{id}
        // View Patient Details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            try
            {
                // Fetch the patient along with related Caregiver and CaregiverNotes
                var patient = await _context.Patient
                                            .Include(p => p.Caregiver)       // Include Caregiver details
                                            .Include(p => p.CaregiverNotes)  // Include Caregiver notes
                                            .FirstOrDefaultAsync(p => p.PatientId == id);

                // Check if the patient exists
                if (patient == null)
                {
                    return NotFound(new { Message = $"Patient with ID {id} not found." });
                }

                // Optional: Return a simplified object (DTO) if needed
                var patientDto = new
                {
                    patient.PatientId,
                    patient.Name,
                    patient.Address,
                    patient.DateOfBirth,
                    patient.MedicalRecord,
                    Caregiver = patient.Caregiver != null ? new
                    {
                        patient.Caregiver.CaregiverId,
                        patient.Caregiver.Name,
                        patient.Caregiver.Specialization
                    } : null,
                    Notes = patient.CaregiverNotes.Select(note => new
                    {
                        note.NoteId,
                        note.NoteContent,
                        note.CreatedAt
                    })
                };

                return Ok(patientDto);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 error response
                return StatusCode(500, new { Message = "An error occurred while fetching the patient details.", Details = ex.Message });
            }
        }


        // PUT: api/Patient/{id}
        // Update Patient Information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDTO updatedPatient)
        {
            if (id != updatedPatient.patientId)
            {
                return BadRequest("Patient ID mismatch.");
            }

            // Check if patient exists before updating
            var existingPatient = await _context.Patient.FindAsync(id);
            if (existingPatient == null)
            {
                return NotFound("Patient not found.");
            }

            // Update patient details
            existingPatient.Name = updatedPatient.Name;
            existingPatient.Address = updatedPatient.Address;
            existingPatient.MedicalRecord = updatedPatient.MedicalRecord;
            existingPatient.DateOfBirth = (DateTime)updatedPatient.DateOfBirth;
            existingPatient.CaregiverId = updatedPatient.CaregiverId;

            _context.Entry(existingPatient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // Successful update with no content in response
        }

        // DELETE: api/Patient/{id}
        // Remove Patient
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemovePatient(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent(); // Successful deletion with no content in response
        }


        // return the user details from Users 
        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            // Fetch the user based on userId from the Identity table
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            // Check if the user exists
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Combine the first name and last name of the user
            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            // Search for a patient whose name matches the user's full name
            var patient = await _context.Patient.FirstOrDefaultAsync(p => p.Name == fullName);

            // Check if the patient record exists
            if (patient == null)
            {
                return NotFound(new { message = "Patient record not found for the user." });
            }

            // Fetch the caregiver based on the CaregiverId from the patient record
            var caregiver = await _context.Caregiver.FirstOrDefaultAsync(c => c.CaregiverId == patient.CaregiverId);

            // Check if the caregiver record exists
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver record not found for the patient." });
            }

            // Return user, patient, and caregiver details
            return Ok(new
            {
                UserDetails = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    user.Email,
                    user.Address,
                    user.PhoneNumber,
                    user.UserType,
                    user.IsActive,
                    user.CreatedAt
                },
                PatientDetails = new
                {
                    patient.PatientId,
                    patient.Name,
                    patient.Address,
                    patient.MedicalRecord,
                    patient.DateOfBirth,
                    patient.CaregiverId
                },
                CaregiverDetails = new
                {
                    caregiver.CaregiverId,
                    caregiver.Name,
                    caregiver.Specialization,
                    caregiver.IsAvailable,
                    caregiver.IsActive
                }
            });
        }
    }
}

