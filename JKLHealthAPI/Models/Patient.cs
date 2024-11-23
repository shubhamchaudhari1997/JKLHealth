using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public string MedicalRecord { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // Make CaregiverId nullable
        public int? CaregiverId { get; set; }

        [ForeignKey("CaregiverId")]
        public Caregiver Caregiver { get; set; }

        public ICollection<CaregiverNote> CaregiverNotes { get; set; } = new List<CaregiverNote>();
    }
}
