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

        // GET: api/Caregiver
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Caregiver>>> GetCaregivers()
        {
            return await _context.Caregiver.Include(c => c.Patients).ToListAsync();
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
        public async Task<IActionResult> PutCaregiver(int id, Caregiver caregiver)
        {
            if (id != caregiver.CaregiverId)
            {
                return BadRequest();
            }

            _context.Entry(caregiver).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaregiverExists(id))
                {
                    return NotFound();
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

        // GET: api/Caregiver/{caregiverId}/patients
        [HttpGet("{caregiverId}/patients")]
        public async Task<IActionResult> GetAssignedPatients(int caregiverId)
        {
            // Check if caregiver exists
            var caregiver = await _context.Caregiver.FindAsync(caregiverId);
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found" });
            }

            // Fetch assigned patients
            var patients = await _context.Patient
                .Where(p => p.CaregiverId == caregiverId)
                .Select(p => new
                {
                    p.PatientId,
                    p.Name,
                    p.Address,
                    p.MedicalRecord,
                    p.DateOfBirth
                })
                .ToListAsync();

            return Ok(new
            {
                Caregiver = caregiver.Name,
                TotalPatients = patients.Count,
                Patients = patients
            });
        }

        // GET: api/Caregiver/{caregiverId}/appointments
        [HttpGet("{caregiverId}/appointments")]
        public async Task<IActionResult> GetAppointmentsByDate(int caregiverId)
        {
            // Verify caregiver exists
            var caregiver = await _context.Caregiver.FindAsync(caregiverId);
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found" });
            }

            // Fetch and group appointments by date
            var appointments = await _context.Appointments
                .Where(a => a.CaregiverId == caregiverId)
                .GroupBy(a => a.AppointmentDate.Date)  // Group by the date part only
                .Select(group => new
                {
                    Date = group.Key,
                    TotalAppointments = group.Count(),
                    Appointments = group.Select(a => new
                    {
                        a.AppointmentId,
                        a.Notes,
                        a.PatientId
                    })
                })
                .ToListAsync();

            return Ok(new
            {
                Caregiver = caregiver.Name,
                AppointmentsByDate = appointments
            });
        }

        [HttpGet("{caregiverId}/today-appointments")]
        public async Task<IActionResult> GetTodayAppointments(int caregiverId)
        {
            // Verify caregiver exists
            var caregiver = await _context.Caregiver.FindAsync(caregiverId);
            if (caregiver == null)
            {
                return NotFound(new { message = "Caregiver not found" });
            }

            // Fetch today's appointments
            var today = DateTime.UtcNow.Date;
            var appointments = await _context.Appointments
                .Where(a => a.CaregiverId == caregiverId && a.AppointmentDate.Date == today)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.Notes,
                    a.PatientId,
                    PatientName = a.Patient.Name
                })
                .ToListAsync();

            if (appointments.Count == 0)
            {
                return Ok(new { message = "No appointments for today" });
            }

            return Ok(new
            {
                Caregiver = caregiver.Name,
                Date = today,
                TotalAppointments = appointments.Count,
                Appointments = appointments
            });
        }

        private bool CaregiverExists(int id)
        {
            return _context.Caregiver.Any(e => e.CaregiverId == id);
        }
    }
}
