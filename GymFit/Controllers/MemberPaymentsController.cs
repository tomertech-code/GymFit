using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Member")]
public class MemberPaymentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MemberPaymentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var payments = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Subscription!).ThenInclude(s => s.MembershipPlan)
            .Where(p => p.Member!.UserId == user.Id)
            .OrderByDescending(p => p.PaymentDate)
            .Take(500)
            .ToListAsync();
        return View(payments);
    }
}
