using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.DTOs;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace GymFit.Services
{

    public class DashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public DashboardService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new DashboardStatsDto();

            var allMembers = await _unitOfWork.Members.GetAllAsync();
            stats.TotalMembers = allMembers.Count();
            stats.ActiveMembers = allMembers.Count(m => m.IsActive);

            var trainers = await _unitOfWork.Trainers.GetAllAsync();
            stats.TotalTrainers = trainers.Count();
            stats.ExpiredMemberships = await _context.Subscriptions.CountAsync(s => s.IsActive && s.EndDate < DateTime.UtcNow);
            stats.TodayEnquiries = await _context.ContactMessages.CountAsync(m => m.CreatedAt >= DateTime.UtcNow.Date);

            stats.TodayAttendance = await _unitOfWork.Attendances.GetTodayAttendanceCountAsync();
            stats.MonthlyRevenue = await _unitOfWork.Payments.GetMonthlyRevenueAsync();

            // Get recent members
            var recentMembers = allMembers
                .OrderByDescending(m => m.JoinDate)
                .Take(5)
                .Select(m => new RecentMemberDto
                {
                    Name = $"{m.User.FirstName} {m.User.LastName}",
                    JoinDate = m.JoinDate,
                    Plan = m.Subscriptions.FirstOrDefault()?.MembershipPlan.Name ?? "No Plan"
                })
                .ToList();

            stats.RecentMembers = recentMembers;

            // Get revenue chart data for last 6 months
            var payments = await _unitOfWork.Payments.GetAllAsync();
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            // Use PaymentStatus directly if it's in the same namespace as Payment entity
            stats.RevenueChart = payments
                .Where(p => p.PaymentDate >= sixMonthsAgo && p.Status.ToString() == "Completed")
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new RevenueChartDto
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Amount = g.Sum(p => p.Amount)
                })
                .OrderBy(r => r.Month)
                .ToList();

            return stats;
        }
    }
}
