using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Domain.Enums;

namespace GymFit.Services
{

    public class MembershipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MembershipPlan>> GetActivePlansAsync()
        {
            return await _unitOfWork.MembershipPlans.GetActivePlansAsync();
        }

        public async Task<bool> SubscribeMemberAsync(int memberId, int planId)
        {
            try
            {
                var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(planId);
                if (plan == null) return false;

                var subscription = new Subscription
                {
                    MemberId = memberId,
                    MembershipPlanId = planId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                    Status = SubscriptionStatus.Active,
                    IsActive = true
                };

                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
