using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Domain.Enums;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Admin,Reception")]
public class MembershipsController : Controller
{
    private readonly ApplicationDbContext _context;

    public MembershipsController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, bool includeExpired = false)
    {
        var query = _context.Subscriptions.AsNoTracking()
            .Include(s => s.Member).ThenInclude(m => m.User)
            .Include(s => s.MembershipPlan)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => (s.Member.User.FirstName + " " + s.Member.User.LastName).Contains(search) || s.MembershipPlan.Name.Contains(search));
        if (!includeExpired)
            query = query.Where(s => s.IsActive);

        ViewBag.Search = search;
        ViewBag.IncludeExpired = includeExpired;
        ViewBag.Members = await _context.Members.AsNoTracking().Where(m => m.IsActive).Include(m => m.User).OrderBy(m => m.User.LastName).ToListAsync();
        ViewBag.Plans = await _context.MembershipPlans.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Price).ToListAsync();
        return View(await query.OrderByDescending(s => s.EndDate).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(MembershipAssignmentViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == model.MemberId && m.IsActive);
        var plan = await _context.MembershipPlans.FirstOrDefaultAsync(p => p.Id == model.MembershipPlanId && p.IsActive);
        if (member is null || plan is null)
            return NotFound();

        var start = model.StartDate.Date;
        var active = await _context.Subscriptions.FirstOrDefaultAsync(s => s.MemberId == member.Id && s.IsActive);
        if (active is not null)
        {
            active.IsActive = false;
            active.Status = SubscriptionStatus.Expired;
        }

        _context.Subscriptions.Add(new Subscription
        {
            MemberId = member.Id,
            MembershipPlanId = plan.Id,
            StartDate = start,
            EndDate = start.AddDays(plan.DurationDays),
            Status = SubscriptionStatus.Active,
            IsActive = true
        });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Membership assigned successfully.";
        return RedirectToAction(nameof(Index));
    }
}
