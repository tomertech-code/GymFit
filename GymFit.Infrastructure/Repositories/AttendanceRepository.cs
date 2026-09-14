using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Infrastructure.Repositories
{

    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Attendance>> GetMemberAttendanceAsync(int memberId)
        {
            return await _context.Attendances
                .Where(a => a.MemberId == memberId)
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();
        }

        public async Task<int> GetTodayAttendanceCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Attendances
                .CountAsync(a => a.CheckInTime.Date == today);
        }
    }
}
