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
            var now = DateTime.UtcNow;
            var startOfToday = now.Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfChart = startOfMonth.AddMonths(-11);

            var stats = new DashboardStatsDto
            {
                TotalMembers = await _context.Members.AsNoTracking().CountAsync(),
                ActiveMembers = await _context.Members.AsNoTracking().CountAsync(m => m.IsActive),
                TotalTrainers = await _context.Trainers.AsNoTracking().CountAsync(),
                ExpiredMemberships = await _context.Subscriptions.AsNoTracking()
                    .CountAsync(s => s.IsActive && s.EndDate < now),
                TodayEnquiries = await _context.ContactMessages.AsNoTracking()
                    .CountAsync(m => m.CreatedAt >= startOfToday && m.CreatedAt < startOfToday.AddDays(1)),
                TodayAttendance = await _context.Attendances.AsNoTracking()
                    .CountAsync(a => a.CheckInTime >= startOfToday && a.CheckInTime < startOfToday.AddDays(1)),
                MonthlyRevenue = await _context.Payments.AsNoTracking()
                    .Where(p => p.PaymentDate >= startOfMonth && p.Status == Domain.Enums.PaymentStatus.Completed)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0m
            };

            stats.RecentMembers = await _context.Members.AsNoTracking()
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.JoinDate)
                .Take(5)
                .Select(m => new RecentMemberDto
                {
                    Name = m.User.FirstName + " " + m.User.LastName,
                    JoinDate = m.JoinDate,
                    Plan = m.Subscriptions
                        .OrderByDescending(s => s.CreatedAt)
                        .Select(s => s.MembershipPlan.Name)
                        .FirstOrDefault() ?? "No Plan"
                })
                .ToListAsync();

            var payments = await _context.Payments.AsNoTracking()
                .Where(p => p.PaymentDate >= startOfChart && p.Status == Domain.Enums.PaymentStatus.Completed)
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Amount = g.Sum(p => p.Amount)
                })
                .ToListAsync();

            stats.RevenueChart = payments
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new RevenueChartDto
                {
                    Month = $"{x.Month}/{x.Year}",
                    Amount = x.Amount
                })
                .ToList();

            return stats;
        }
    }
}
