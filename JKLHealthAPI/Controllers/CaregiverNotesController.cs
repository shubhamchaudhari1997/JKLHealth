using JKLHealthAPI.Data;
using JKLHealthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JKLHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaregiverNotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CaregiverNotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateCaregiverNote")]
        public async Task<IActionResult> AddCaregiverNote([FromBody] CaregiverNoteDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var note = new CaregiverNote
            {
                NoteContent = model.noteContent,
                PatientId = model.PatientId,
                CaregiverId = model.CaregiverId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.CaregiverNotes.AddAsync(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Caregiver note added successfully." });
        }
    }
}
