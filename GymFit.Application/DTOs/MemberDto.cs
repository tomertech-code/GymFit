using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Application.DTOs
{

    public class MemberDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public string? AssignedTrainerName { get; set; }
        public string? CurrentPlan { get; set; }
        public int PrimaryBranchId { get; set; }
        public int AssignedTrainerId { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        // Computed
        public string FullName => $"{FirstName} {LastName}";
    }
}
