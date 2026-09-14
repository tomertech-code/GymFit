using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Enums;

namespace GymFit.Domain.Entities
{

    public class MembershipPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public MembershipType Type { get; set; }
        public string Features { get; set; } = string.Empty;

        // Multi-branch access
        public bool AllowsMultiBranchAccess { get; set; } = false;
        public int? MaxBranchesAllowed { get; set; } // null = unlimited

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}