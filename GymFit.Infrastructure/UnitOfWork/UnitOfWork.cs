using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Infrastructure.Data;
using GymFit.Infrastructure.Repositories;

namespace GymFit.Infrastructure.UnitOfWork
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IMemberRepository Members { get; }
        public ITrainerRepository Trainers { get; }
        public IMembershipRepository MembershipPlans { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public IAttendanceRepository Attendances { get; }
        public IPaymentRepository Payments { get; }
        public IWorkoutPlanRepository WorkoutPlans { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Members = new MemberRepository(_context);
            Trainers = new TrainerRepository(_context);
            MembershipPlans = new MembershipRepository(_context);
            Subscriptions = new SubscriptionRepository(_context);
            Attendances = new AttendanceRepository(_context);
            Payments = new PaymentRepository(_context);
            WorkoutPlans = new WorkoutPlanRepository(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
