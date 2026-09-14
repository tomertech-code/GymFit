using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Application.DTOs
{

    public class DashboardStatsDto
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int TotalTrainers { get; set; }
        public int ExpiredMemberships { get; set; }
        public int TodayEnquiries { get; set; }
        public int TodayAttendance { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ExpiringSubscriptions { get; set; }
        public List<RecentMemberDto> RecentMembers { get; set; } = new();
        public List<RevenueChartDto> RevenueChart { get; set; } = new();
    }

    public class RecentMemberDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public string Plan { get; set; } = string.Empty;
    }

    public class RevenueChartDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
