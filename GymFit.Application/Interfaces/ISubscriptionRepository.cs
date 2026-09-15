using GymFit.Domain.Entities;

namespace GymFit.Application.Interfaces;

public interface ISubscriptionRepository : IGenericRepository<Subscription>
{
    Task<Subscription?> GetActiveByMemberIdAsync(int memberId);
}
