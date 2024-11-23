using JKLHealthAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Admin/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetAdminDashboard()
        {
            // Total number of caregivers
            int totalCaregivers = await _context.Caregiver.CountAsync();

            // Number of active caregivers
            int activeCaregivers = await _context.Caregiver.CountAsync(c => c.IsActive);

            // Number of caregivers currently available (not assigned)
            int availableCaregivers = await _context.Caregiver.CountAsync(c => c.IsAvailable);

            // Number of caregivers currently assigned (those who are not available)
            int assignedCaregivers = totalCaregivers - availableCaregivers;

            // Total number of patients
            int totalPatients = await _context.Patient.CountAsync();

            // Number of pending appointments (future appointments)
            int pendingAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate > DateTime.UtcNow);

            // Return the statistics as a JSON object
            return Ok(new
            {
                TotalCaregivers = totalCaregivers,
                ActiveCaregivers = activeCaregivers,
                AvailableCaregivers = availableCaregivers,
                AssignedCaregivers = assignedCaregivers,
                TotalPatients = totalPatients,
                PendingAppointments = pendingAppointments
            });
        }
    }
}
