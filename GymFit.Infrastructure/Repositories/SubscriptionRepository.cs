using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Infrastructure.Repositories;

public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Subscription?> GetActiveByMemberIdAsync(int memberId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.MemberId == memberId && s.IsActive);
    }
}
