﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JKLHealthAPI.Models
{
    public class Caregiver
    {
        [Key]
        public int CaregiverId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; }

        public bool IsAvailable { get; set; }

        // Navigation property to link patients
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}