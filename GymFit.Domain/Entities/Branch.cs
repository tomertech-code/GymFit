using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // Unique code like "GF-MUM-01"
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public TimeSpan OpeningTime { get; set; } = new TimeSpan(6, 0, 0); // 6 AM
        public TimeSpan ClosingTime { get; set; } = new TimeSpan(22, 0, 0); // 10 PM
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
        public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<BranchEquipment> Equipment { get; set; } = new List<BranchEquipment>();
    }
}
