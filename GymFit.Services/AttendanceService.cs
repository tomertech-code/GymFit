using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
                var member = await _unitOfWork.Members.GetByIdAsync(memberId);
                if (member is null || !member.IsActive || member.PrimaryBranchId <= 0)
                    return false;

                var existing = await _unitOfWork.Attendances.GetOpenAttendanceAsync(memberId);
                if (existing is not null)
                    return false;

                var attendance = new Attendance
                {
                    MemberId = memberId,
                    BranchId = member.PrimaryBranchId,
                    CheckInTime = DateTime.UtcNow
                };

                await _unitOfWork.Attendances.AddAsync(attendance);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (DbUpdateException)
            {
                // The database unique filtered index is the final concurrency guard.
                return false;
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
