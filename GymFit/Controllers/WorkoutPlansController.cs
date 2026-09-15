using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Admin,Trainer,Member")]
public class WorkoutPlansController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WorkoutPlansController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var query = _context.WorkoutPlans.AsNoTracking()
            .Include(p => p.Member).ThenInclude(m => m.User)
            .Include(p => p.Trainer).ThenInclude(t => t.User)
            .Include(p => p.Exercises)
            .Where(p => p.IsActive);

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        if (User.IsInRole("Member"))
            query = query.Where(p => p.Member.UserId == user.Id);
        else if (User.IsInRole("Trainer"))
        {
            query = query.Where(p => p.Trainer.UserId == user.Id);
            ViewBag.AssignedMembers = await _context.Members.AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.AssignedTrainer != null && m.AssignedTrainer.UserId == user.Id && m.IsActive)
                .OrderBy(m => m.User.LastName)
                .ToListAsync();
        }

        return View(await query.OrderByDescending(p => p.StartDate).ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutPlanViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.GetUserAsync(User);
        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == model.MemberId && m.IsActive);
        if (user is null || member is null)
            return NotFound();

        var trainer = User.IsInRole("Trainer")
            ? await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id && t.IsActive)
            : await _context.Trainers.FirstOrDefaultAsync(t => t.Id == member.AssignedTrainerId && t.IsActive);

        if (trainer is null || (User.IsInRole("Trainer") && member.AssignedTrainerId != trainer.Id))
            return Forbid();

        _context.WorkoutPlans.Add(new WorkoutPlan
        {
            MemberId = member.Id,
            TrainerId = trainer.Id,
            Name = model.Name,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            IsActive = true
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutPlanViewModel model)
    {
        if (id <= 0 || !ModelState.IsValid)
            return BadRequest(new { success = false, message = "Invalid workout plan data." });

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var plan = await _context.WorkoutPlans
            .Include(p => p.Trainer)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        if (plan is null)
            return NotFound();

        if (User.IsInRole("Trainer") && plan.Trainer.UserId != user.Id)
            return Forbid();

        if (model.EndDate.HasValue && model.EndDate.Value < model.StartDate)
            return BadRequest(new { success = false, message = "End date cannot be before start date." });

        plan.Name = model.Name.Trim();
        plan.Description = model.Description?.Trim();
        plan.StartDate = model.StartDate;
        plan.EndDate = model.EndDate;
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Workout plan updated successfully." });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var plan = await _context.WorkoutPlans
            .Include(p => p.Trainer)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        if (plan is null)
            return NotFound();

        if (User.IsInRole("Trainer") && plan.Trainer.UserId != user.Id)
            return Forbid();

        plan.IsActive = false;
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Workout plan deactivated successfully." });
    }

}
