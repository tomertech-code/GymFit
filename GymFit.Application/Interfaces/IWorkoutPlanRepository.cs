using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Entities;

namespace GymFit.Application.Interfaces
{

    public interface IWorkoutPlanRepository : IGenericRepository<WorkoutPlan>
    {
        Task<IEnumerable<WorkoutPlan>> GetMemberWorkoutPlansAsync(int memberId);
    }
}
