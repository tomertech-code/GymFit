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

    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Member?> GetMemberWithDetailsAsync(int id)
        {
            return await _context.Members
                .Include(m => m.User)
                .Include(m => m.AssignedTrainer)
                    .ThenInclude(t => t.User)
                .Include(m => m.Subscriptions)
                    .ThenInclude(s => s.MembershipPlan)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Member>> GetActiveMembersAsync()
        {
            return await _context.Members
                .Include(m => m.User)
                .Include(m => m.AssignedTrainer)
                    .ThenInclude(t => t.User)
                .Where(m => m.IsActive)
                .ToListAsync();
        }

        public Task<int> CountActiveAsync() => _context.Members.AsNoTracking().CountAsync(m => m.IsActive);

        public async Task<Member?> GetMemberByUserIdAsync(string userId)
        {
            return await _context.Members
                .Include(m => m.User)
                .Include(m => m.Subscriptions)
                    .ThenInclude(s => s.MembershipPlan)
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }
    }
}
