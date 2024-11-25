namespace JKLHealthAPI.Models
{
    public class UpdatePatientDTO
    {
        public int patientId {  get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string MedicalRecord { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CaregiverId { get; set; }
    }
}
