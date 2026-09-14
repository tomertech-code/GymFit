using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{


    public class Trainer
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Branch Assignment
        public int PrimaryBranchId { get; set; }

        public string Specialization { get; set; } = string.Empty;
        public string Certifications { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; } = true;

        // Multi-branch availability
        public bool WorksAtMultipleBranches { get; set; } = false;

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Branch PrimaryBranch { get; set; } = null!;
        public virtual ICollection<Member> AssignedMembers { get; set; } = new List<Member>();
        public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
        public virtual ICollection<TrainerBranchAssignment> BranchAssignments { get; set; } = new List<TrainerBranchAssignment>();
    }
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Specialization { get; set; } = "";
        public int ExperienceYears { get; set; }
        public int MemberCount { get; set; }
        public bool IsActive { get; set; }
        public string BranchName { get; set; } = "";
    }
}
