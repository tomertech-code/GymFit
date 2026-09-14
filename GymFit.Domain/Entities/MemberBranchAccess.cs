using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{

    public class MemberBranchAccess
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BranchId { get; set; }
        public bool HasAccess { get; set; } = true;
        public DateTime GrantedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }

        public virtual Member Member { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
    }
}
