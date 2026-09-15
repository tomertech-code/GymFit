using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Admin,Trainer")]
public class ExercisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExercisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int workoutPlanId, string name, string? description, int sets, int reps, string? restTime, string? notes, int dayOfWeek)
    {
        if (workoutPlanId <= 0 || string.IsNullOrWhiteSpace(name) || sets <= 0 || reps <= 0 || dayOfWeek is < 0 or > 6)
            return BadRequest(new { success = false, message = "Invalid exercise data." });

        var plan = await _context.WorkoutPlans
            .Include(p => p.Trainer)
            .FirstOrDefaultAsync(p => p.Id == workoutPlanId && p.IsActive);
        if (plan is null)
            return NotFound();

        if (User.IsInRole("Trainer"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null || plan.Trainer.UserId != user.Id)
                return Forbid();
        }

        _context.Exercises.Add(new Exercise
        {
            WorkoutPlanId = plan.Id,
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Sets = sets,
            Reps = reps,
            RestTime = restTime?.Trim(),
            Notes = notes?.Trim(),
            DayOfWeek = dayOfWeek
        });
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Exercise added successfully." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var exercise = await _context.Exercises
            .Include(e => e.WorkoutPlan).ThenInclude(p => p.Trainer)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (exercise is null) return NotFound();

        if (User.IsInRole("Trainer"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null || exercise.WorkoutPlan.Trainer.UserId != user.Id)
                return Forbid();
        }

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Exercise deleted successfully." });
    }
}
