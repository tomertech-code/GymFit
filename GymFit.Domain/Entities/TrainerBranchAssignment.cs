using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{

    public class TrainerBranchAssignment
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public int BranchId { get; set; }
        public string? WorkingDays { get; set; } // Mon,Wed,Fri
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Trainer Trainer { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
    }
}
