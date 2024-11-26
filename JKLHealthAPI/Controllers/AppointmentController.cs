using JKLHealthAPI.Data;
using JKLHealthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments.Include(a => a.Patient).Include(a => a.Caregiver).ToListAsync();
        }

        [HttpGet("patient/{patientId}")]
public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByPatientId(int patientId)
{
    // Fetch appointments for the given PatientId
    var appointments = await _context.Appointments
        .Where(a => a.PatientId == patientId)
        .Include(a => a.Patient)
        .Include(a => a.Caregiver)
        .ToListAsync();

    // Check if any appointments exist
    if (!appointments.Any())
    {
        return NotFound(new { message = "No appointments found for the specified patient." });
    }

    // Map the appointments to a simplified DTO if necessary
    var appointmentDtos = appointments.Select(a => new
    {
        a.AppointmentId,
        a.AppointmentDate,
        status = a.AppointmentType,
        a.Notes,
        CaregiverName = a.Caregiver?.Name,
        PatientName = a.Patient?.Name
    });

    return Ok(appointmentDtos);
}


        // GET: api/Appointment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Caregiver)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        [HttpPost("PostAppointment")]
        public async Task<ActionResult<Appointment>> PostAppointment([FromBody] AppointmentDto appointmentDto)
        {
            // Map the DTO to the Appointment entity
            var appointment = new Appointment
            {
                AppointmentDate = appointmentDto.AppointmentDate,
                AppointmentType = appointmentDto.Status,  // Map Status to AppointmentType
                Notes = appointmentDto.Notes,
                PatientId = appointmentDto.PatientId,
                CaregiverId = appointmentDto.CaregiverId
            };

            // Add and save the new appointment
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
        }

        // PUT: api/Appointment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // PUT: api/Appointment/UpdateStatus/{appointmentId}
        [HttpPut("UpdateStatus/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, [FromBody] UpdateStatusRequest request)
        {
            // Find the appointment by ID
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            // Check if the appointment exists
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            // Update the status
            appointment.AppointmentType = request.Status;

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Appointment status updated successfully." });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { error = "Database error occurred.", details = dbEx.Message });
            }
        }


        // DELETE: api/Appointment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
