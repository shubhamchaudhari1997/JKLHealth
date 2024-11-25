using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models
{
    public class CaregiverNoteDTO
    {
        [Required]
        public string noteContent { get; set; } // or NoteContent
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int CaregiverId { get; set; }
    }
}
