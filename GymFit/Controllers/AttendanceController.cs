using GymFit.Infrastructure.Data;
using GymFit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymFit.Domain.Entities;

namespace GymFit.Web.Controllers
{


    [Authorize(Roles = "Admin,Reception")]
    public class AttendanceController : Controller
    {
        private readonly AttendanceService _attendanceService;
        private readonly ApplicationDbContext _context;

        public AttendanceController(AttendanceService attendanceService, ApplicationDbContext context)
        {
            _attendanceService = attendanceService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetToday()
        {
            var today = DateTime.Today;
            var attendance = await _context.Attendances
                .Include(a => a.Member)
                    .ThenInclude(m => m.User)
                .Where(a => a.CheckInTime.Date == today)
                .OrderByDescending(a => a.CheckInTime)
                .Select(a => new
                {
                    a.Id,
                    a.MemberId,
                    MemberName = a.Member.User.FirstName + " " + a.Member.User.LastName,
                    CheckInTime = a.CheckInTime.ToString("hh:mm tt"),
                    CheckOutTime = a.CheckOutTime.HasValue ? a.CheckOutTime.Value.ToString("hh:mm tt") : null,
                    Duration = a.CheckOutTime.HasValue ?
                        (a.CheckOutTime.Value - a.CheckInTime).TotalMinutes.ToString("0") + " mins" : "Active",
                    Status = a.CheckOutTime.HasValue ? "Completed" : "Active"
                })
                .ToListAsync();

            return Json(attendance);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _context.Members
                .Include(m => m.User)
                .Where(m => m.IsActive)
                .Select(m => new
                {
                    m.Id,
                    Name = m.User.FirstName + " " + m.User.LastName
                })
                .ToListAsync();

            return Json(members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int memberId)
        {
            var result = await _attendanceService.CheckInMemberAsync(memberId);
            if (result)
                return Json(new { success = true, message = "Member checked in successfully" });

            return Json(new { success = false, message = "Failed to check in" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int attendanceId)
        {
            var result = await _attendanceService.CheckOutMemberAsync(attendanceId);
            if (result)
                return Json(new { success = true, message = "Member checked out successfully" });

            return Json(new { success = false, message = "Failed to check out" });
        }
    }

}
