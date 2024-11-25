using JKLHealthAPI.Data;
using JKLHealthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaregiverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CaregiverController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCaregivers")]
        public async Task<ActionResult<IEnumerable<Caregiver>>> GetCaregivers()
        {
            try
            {
                var caregivers = await _context.Caregiver.ToListAsync();
                return Ok(caregivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching caregivers", Details = ex.Message });
            }
        }

        // GET: api/Caregiver/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Caregiver>> GetCaregiver(int id)
        {
            var caregiver = await _context.Caregiver
                .Include(c => c.Patients)
                .FirstOrDefaultAsync(c => c.CaregiverId == id);

            if (caregiver == null)
            {
                return NotFound();
            }

            return caregiver;
        }

        // POST: api/Caregiver
        [HttpPost]
        public async Task<ActionResult<Caregiver>> PostCaregiver(Caregiver caregiver)
        {
            _context.Caregiver.Add(caregiver);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCaregiver), new { id = caregiver.CaregiverId }, caregiver);
        }

        // PUT: api/Caregiver/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCaregiver(int id, CaregiverUpdateDto caregiverDto)
        {
            // Find the existing caregiver
            var caregiver = await _context.Caregiver.FindAsync(id);
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found." });
            }

            // Find the user in the Identity table
            var user = await _context.Users.FindAsync(caregiver.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User details not found." });
            }

            // Update caregiver entity fields
            caregiver.Name = caregiverDto.Name;
            caregiver.Specialization = caregiverDto.Specialization;

            // Update user entity fields (Identity table)
            user.PhoneNumber = caregiverDto.PhoneNumber;
            user.Address = caregiverDto.Address;  // Ensure Address is a custom property in ApplicationUser

            // Save changes to both entities
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaregiverExists(id))
                {
                    return NotFound(new { message = "Concurrency error: Caregiver not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Caregiver/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaregiver(int id)
        {
            var caregiver = await _context.Caregiver.FindAsync(id);
            if (caregiver == null)
            {
                return NotFound();
            }

            _context.Caregiver.Remove(caregiver);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Caregiver/{id}/Patients
        [HttpGet("{id}/patients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetCaregiverPatients(int id)
        {
            var caregiver = await _context.Caregiver
                .Include(c => c.Patients)
                .FirstOrDefaultAsync(c => c.CaregiverId == id);

            if (caregiver == null)
            {
                return NotFound();
            }

            return Ok(caregiver.Patients);
        }

        // GET: api/Caregiver/{id}/Notes
        [HttpGet("{id}/notes")]
        public async Task<ActionResult<IEnumerable<CaregiverNote>>> GetCaregiverNotes(int id)
        {
            var notes = await _context.CaregiverNotes
                .Where(n => n.CaregiverId == id)
                .ToListAsync();

            if (notes == null || !notes.Any())
            {
                return NotFound("No notes found for this caregiver.");
            }

            return Ok(notes);
        }

        // GET: api/caregiver/details/{userId}
        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetCaregiverByUserId(string userId)
        {
            // Fetch caregiver based on UserId
            var caregiver = await _context.Caregiver
                                          .Include(c => c.Patients)  // Include related data if necessary
                                          .FirstOrDefaultAsync(c => c.UserId == userId);

            // Check if caregiver exists
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found for the provided UserId." });
            }

            // Fetch user details from ApplicationUser (AspNetUsers table)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            // Check if user exists
            if (user == null)
            {
                return NotFound(new { message = "User details not found for the provided UserId." });
            }


            // Create and return a structured response
            return Ok(new
            {
                CaregiverId = caregiver.CaregiverId,
                Name = caregiver.Name,
                Specialization = caregiver.Specialization,
                IsAvailable = caregiver.IsAvailable,
                IsActive = caregiver.IsActive,
                TotalPatients = caregiver.Patients?.Count ?? 0,  // Total related patients count
                PhoneNumber = user.PhoneNumber,  // Fetch from AspNetUsers
                Address = user.Address
            });
        }


        // GET: api/Caregiver/patients/{userId}
        [HttpGet("patients/{userId}")]
        public async Task<IActionResult> GetPatientsByUserId(string userId)
        {
            // Find the caregiver by UserId
            var caregiver = await _context.Caregiver
                                          .FirstOrDefaultAsync(c => c.UserId == userId);

            // Check if caregiver exists
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found for the provided UserId." });
            }

            // Fetch patients assigned to this caregiver
            var patients = await _context.Patient
                                         .Where(p => p.CaregiverId == caregiver.CaregiverId)
                                         .Select(p => new
                                         {
                                             p.PatientId,
                                             p.Name,
                                             p.Address,
                                             p.MedicalRecord,
                                             p.DateOfBirth
                                         })
                                         .ToListAsync();

            // Return the caregiver's name and the assigned patients
            return Ok(new
            {
                Caregiver = caregiver.Name,
                TotalPatients = patients.Count,
                Patients = patients
            });
        }

        // GET: api/Caregiver/appointments/{userId}
        [HttpGet("appointments/{userId}")]
        public async Task<IActionResult> GetAppointmentsByUserId(string userId)
        {
            // Find the caregiver by UserId
            var caregiver = await _context.Caregiver.FirstOrDefaultAsync(c => c.UserId == userId);

            // Verify caregiver exists
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found for the provided UserId." });
            }

            // Fetch and group appointments by date
            var appointments = await _context.Appointments
                .Where(a => a.CaregiverId == caregiver.CaregiverId)
                .GroupBy(a => a.AppointmentDate.Date)  // Group by the date part only
                .Select(group => new
                {
                    Date = group.Key,
                    TotalAppointments = group.Count(),
                    Appointments = group.Select(a => new
                    {
                        a.AppointmentId,
                        a.PatientId,
                        a.Notes,
                        a.AppointmentDate
                    })
                })
                .ToListAsync();

            // Return structured response
            return Ok(new
            {
                Caregiver = caregiver.Name,
                TotalAppointments = appointments.Sum(a => a.TotalAppointments),
                AppointmentsByDate = appointments
            });
        }


        // GET: api/Caregiver/today-appointments/{userId}
        [HttpGet("today-appointments/{userId}")]
        public async Task<IActionResult> GetTodayAppointmentsByUserId(string userId)
        {
            // Find the caregiver by UserId
            var caregiver = await _context.Caregiver.FirstOrDefaultAsync(c => c.UserId == userId);

            // Verify caregiver exists
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found for the provided UserId." });
            }

            // Fetch today's appointments
            var today = DateTime.UtcNow.Date;
            var appointments = await _context.Appointments
                .Where(a => a.CaregiverId == caregiver.CaregiverId && a.AppointmentDate.Date == today)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.Notes,
                    a.PatientId,
                    PatientName = a.Patient.Name
                })
                .ToListAsync();

            // Return a message if no appointments are found
            if (!appointments.Any())
            {
                return Ok(new { message = "No appointments for today." });
            }

            // Return structured response
            return Ok(new
            {
                Caregiver = caregiver.Name,
                Date = today,
                TotalAppointments = appointments.Count,
                Appointments = appointments
            });
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] UpdateAvailabilityDto availabilityDto)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the caregiver by ID
            var caregiver = await _context.Caregiver.FindAsync(id);
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found." });
            }

            // Update the availability status
            caregiver.IsAvailable = availabilityDto.IsAvailable;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it based on your needs
                return StatusCode(500, new { message = "An error occurred while updating availability.", details = ex.Message });
            }

            // Return success response
            return Ok(new
            {
                message = "Availability updated successfully.",
                caregiverId = caregiver.CaregiverId,
                isAvailable = caregiver.IsAvailable
            });
        }



        private bool CaregiverExists(int id)
        {
            return _context.Caregiver.Any(e => e.CaregiverId == id);
        }
    }
}
