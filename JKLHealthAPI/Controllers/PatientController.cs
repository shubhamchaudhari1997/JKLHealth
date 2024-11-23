using JKLHealthAPI.Data;
using JKLHealthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patient
                                         .Include(p => p.Caregiver) // Include Caregiver information
                                         .Include(p => p.CaregiverNotes) // Include Caregiver notes
                                         .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            return patient;
        }

        // PUT: api/Patient/{id}
        // Update Patient Information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, Patient patient)
        {
            if (id != patient.PatientId)
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
            existingPatient.Name = patient.Name;
            existingPatient.Address = patient.Address;
            existingPatient.MedicalRecord = patient.MedicalRecord;
            existingPatient.DateOfBirth = patient.DateOfBirth;
            existingPatient.CaregiverId = patient.CaregiverId;

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
    }
}
