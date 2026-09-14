using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Member")]
public class MemberAttendanceController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MemberAttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        var records = await _context.Attendances.AsNoTracking()
            .Where(a => a.Member.UserId == user.Id)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();
        return View(records);
    }
}
