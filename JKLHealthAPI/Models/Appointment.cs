using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }  // The date and time of the appointment
       
        public string AppointmentType { get; set; }  // e.g., Consultation, Follow-up, Check-up

        //  we need to use AppointmentType as status
        public string Notes { get; set; }  // Optional notes regarding the appointment

        // Foreign key to the Patient
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        // Foreign key to the Caregiver
        public int CaregiverId { get; set; }

        [ForeignKey("CaregiverId")]
        public Caregiver Caregiver { get; set; }
    }

}
