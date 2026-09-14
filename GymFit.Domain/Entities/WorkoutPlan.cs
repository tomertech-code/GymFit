using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{

    public class WorkoutPlan
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int TrainerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Member Member { get; set; } = null!;
        public virtual Trainer Trainer { get; set; } = null!;
        public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
