using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;

namespace GymFit.Services
{

    public class AttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CheckInMemberAsync(int memberId)
        {
            try
            {
                var attendance = new Attendance
                {
                    MemberId = memberId,
                    CheckInTime = DateTime.UtcNow
                };

                await _unitOfWork.Attendances.AddAsync(attendance);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckOutMemberAsync(int attendanceId)
        {
            try
            {
                var attendance = await _unitOfWork.Attendances.GetByIdAsync(attendanceId);
                if (attendance == null || attendance.CheckOutTime != null)
                    return false;

                attendance.CheckOutTime = DateTime.UtcNow;
                _unitOfWork.Attendances.Update(attendance);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Attendance>> GetMemberAttendanceAsync(int memberId)
        {
            return await _unitOfWork.Attendances.GetMemberAttendanceAsync(memberId);
        }

        public async Task<int> GetTodayAttendanceCountAsync()
        {
            return await _unitOfWork.Attendances.GetTodayAttendanceCountAsync();
        }
    }
}
