using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Application.Interfaces
{

    public interface IUnitOfWork : IDisposable
    {
        IMemberRepository Members { get; }
        ITrainerRepository Trainers { get; }
        IMembershipRepository MembershipPlans { get; }
        ISubscriptionRepository Subscriptions { get; }
        IAttendanceRepository Attendances { get; }
        IPaymentRepository Payments { get; }
        IWorkoutPlanRepository WorkoutPlans { get; }
        Task<int> SaveAsync();
    }
}
