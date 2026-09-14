using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Entities
{

    public class Exercise
    {
        public int Id { get; set; }
        public int WorkoutPlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Reps { get; set; }
        public string? RestTime { get; set; }
        public string? Notes { get; set; }
        public int DayOfWeek { get; set; }

        public virtual WorkoutPlan WorkoutPlan { get; set; } = null!;
    }
}
