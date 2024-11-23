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

        private bool CaregiverExists(int id)
        {
            return _context.Caregiver.Any(e => e.CaregiverId == id);
        }
    }
}
