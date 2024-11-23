using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models
{
    public class CaregiverNote
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        public string NoteContent { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Patient
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        // Foreign key to Caregiver (optional, if you want to track which caregiver added the note)
        public int CaregiverId { get; set; }

        [ForeignKey("CaregiverId")]
        public Caregiver Caregiver { get; set; }
    }
}
