using GymFit.Application.ViewModels;
using GymFit.Domain.Enums;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Reception")]
public class ReceptionDashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReceptionDashboardController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var today = DateTime.UtcNow.Date;
        var now = DateTime.UtcNow;
        var model = new ReceptionDashboardViewModel
        {
            TodayCheckIns = await _context.Attendances.CountAsync(a => a.CheckInTime >= today),
            ActiveMembers = await _context.Members.CountAsync(m => m.IsActive),
            ExpiredMemberships = await _context.Subscriptions.CountAsync(s => s.EndDate < now && s.IsActive),
            NewEnquiries = await _context.ContactMessages.CountAsync(m => !m.IsRead),
            TodayPayments = await _context.Payments.CountAsync(p => p.PaymentDate >= today && p.Status == PaymentStatus.Completed),
            ExpiringSoon = await _context.Subscriptions.CountAsync(s => s.IsActive && s.EndDate >= now && s.EndDate <= now.AddDays(30))
        };

        return View(model);
    }
}
