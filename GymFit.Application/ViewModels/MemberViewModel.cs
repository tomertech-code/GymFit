using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Application.ViewModels
{

    public class MemberViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string EmergencyContact { get; set; } = string.Empty;

        public string? MedicalConditions { get; set; }

        [Display(Name = "Assigned Trainer")]
        public int? AssignedTrainerId { get; set; }

        [Display(Name = "Membership Plan")]
        public int? MembershipPlanId { get; set; }
    }

}
