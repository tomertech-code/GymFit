using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Infrastructure.Repositories
{

    public class WorkoutPlanRepository : GenericRepository<WorkoutPlan>, IWorkoutPlanRepository
    {
        public WorkoutPlanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WorkoutPlan>> GetMemberWorkoutPlansAsync(int memberId)
        {
            return await _context.WorkoutPlans
                .Include(wp => wp.Trainer)
                    .ThenInclude(t => t.User)
                .Include(wp => wp.Exercises)
                .Where(wp => wp.MemberId == memberId)
                .OrderByDescending(wp => wp.CreatedAt)
                .ToListAsync();
        }
    }
}
