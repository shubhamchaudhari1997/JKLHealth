using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models
{
    public class AppointmentDto
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }  // Date and time of the appointment

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }  // Appointment status, mapped from AppointmentType

        [MaxLength(500)]
        public string Notes { get; set; }  // Optional notes regarding the appointment

        [Required]
        public int PatientId { get; set; }  // Foreign key to the Patient

        [Required]
        public int CaregiverId { get; set; }  // Foreign key to the Caregiver
    }
}
