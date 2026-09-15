using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Entities;

namespace GymFit.Application.Interfaces
{

    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<Attendance?> GetOpenAttendanceAsync(int memberId);
        Task<IEnumerable<Attendance>> GetMemberAttendanceAsync(int memberId);
        Task<int> GetTodayAttendanceCountAsync();
    }

}
