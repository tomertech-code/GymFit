using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{


    public class Attendance
    {
        public int Id { get; set; }
        public int MemberId { get; set; }

        // Branch tracking
        public int BranchId { get; set; }

        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? Notes { get; set; }

        public virtual Member Member { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
    }
}
