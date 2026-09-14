using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Enums;

namespace GymFit.Domain.Entities
{

    public class Payment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }

        // Branch tracking
        public int BranchId { get; set; }

        public int? SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }

        public virtual Member Member { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
        public virtual Subscription? Subscription { get; set; }
    }
}
