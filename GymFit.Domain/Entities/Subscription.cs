using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Enums;

namespace GymFit.Domain.Entities
{

    public class Subscription
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int MembershipPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Member Member { get; set; } = null!;
        public virtual MembershipPlan MembershipPlan { get; set; } = null!;
    }
}
