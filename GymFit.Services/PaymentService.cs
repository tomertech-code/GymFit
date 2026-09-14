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

    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ProcessPaymentAsync(int memberId, int subscriptionId, decimal amount, string paymentMethod)
        {
            try
            {
                var payment = new Payment
                {
                    MemberId = memberId,
                    SubscriptionId = subscriptionId,
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = paymentMethod,
                    Status = PaymentStatus.Completed,
                    TransactionId = Guid.NewGuid().ToString()
                };

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
