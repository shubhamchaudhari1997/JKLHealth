using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models
{
    public class CaregiverUpdateDto
    {
        public string Name { get; set; }

    
        public string Specialization { get; set; }

    
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
    }
}
