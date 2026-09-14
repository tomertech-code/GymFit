using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{

    public class Member
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Branch Assignment
        public int PrimaryBranchId { get; set; } // Home branch

        public string Address { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public string? MedicalConditions { get; set; }
        public int? AssignedTrainerId { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Multi-branch access
        public bool HasMultiBranchAccess { get; set; } = false;

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Branch PrimaryBranch { get; set; } = null!;
        public virtual Trainer? AssignedTrainer { get; set; }
        public virtual ICollection<MemberBranchAccess> BranchAccesses { get; set; } = new List<MemberBranchAccess>();
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
