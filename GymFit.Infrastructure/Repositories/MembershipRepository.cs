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

    public class MembershipRepository : GenericRepository<MembershipPlan>, IMembershipRepository
    {
        public MembershipRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MembershipPlan>> GetActivePlansAsync()
        {
            return await _context.MembershipPlans
                .Where(mp => mp.IsActive)
                .OrderBy(mp => mp.Price)
                .ToListAsync();
        }
    }
}
