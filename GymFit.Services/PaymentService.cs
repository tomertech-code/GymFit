using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Domain.Enums;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Services
{

    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public PaymentService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<bool> ProcessPaymentAsync(int memberId, int subscriptionId, decimal amount, string paymentMethod)
        {
            try
            {
                if (memberId <= 0 || subscriptionId <= 0 || amount <= 0 || string.IsNullOrWhiteSpace(paymentMethod))
                    return false;

                var member = await _unitOfWork.Members.GetByIdAsync(memberId);
                if (member is null || !member.IsActive || member.PrimaryBranchId <= 0)
                    return false;

                var subscription = await _context.Subscriptions.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.MemberId == memberId && s.IsActive && s.Status == SubscriptionStatus.Active);
                if (subscription is null)
                    return false;

                var payment = new Payment
                {
                    MemberId = memberId,
                    SubscriptionId = subscriptionId,
                    BranchId = member.PrimaryBranchId,
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = paymentMethod,
                    Status = PaymentStatus.Completed,
                    TransactionId = Guid.NewGuid().ToString()
                };

                if (payment.BranchId <= 0)
                    return false;

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Payment>> GetMemberPaymentsAsync(int memberId)
        {
            return await _unitOfWork.Payments.GetMemberPaymentsAsync(memberId);
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            return await _unitOfWork.Payments.GetMonthlyRevenueAsync();
        }
    }
}
