using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Domain.Enums;

namespace GymFit.Services;

public class MembershipService
{
    private readonly IUnitOfWork _unitOfWork;

    public MembershipService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<MembershipPlan>> GetActivePlansAsync()
        => await _unitOfWork.MembershipPlans.GetActivePlansAsync();

    public async Task<bool> SubscribeMemberAsync(int memberId, int planId)
    {
        try
        {
            var member = await _unitOfWork.Members.GetByIdAsync(memberId);
            var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(planId);
            if (member is null || !member.IsActive || plan is null || !plan.IsActive || plan.DurationDays <= 0)
                return false;

            var now = DateTime.UtcNow;
            var active = await _unitOfWork.Subscriptions.GetActiveByMemberIdAsync(memberId);
            if (active is not null)
            {
                active.IsActive = false;
                active.Status = SubscriptionStatus.Expired;
                _unitOfWork.Subscriptions.Update(active);
            }

            var subscription = new Subscription
            {
                MemberId = memberId,
                MembershipPlanId = planId,
                StartDate = now,
                EndDate = now.AddDays(plan.DurationDays),
                Status = SubscriptionStatus.Active,
                IsActive = true,
                CreatedAt = now
            };

            await _unitOfWork.Subscriptions.AddAsync(subscription);
            await _unitOfWork.SaveAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
