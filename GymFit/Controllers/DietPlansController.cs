using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Admin,Reception,Trainer,Member")]
public class DietPlansController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public DietPlansController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }

    public async Task<IActionResult> Index()
    {
        var user = await _users.GetUserAsync(User); if (user == null) return Challenge();
        var q = _db.DietPlans.AsNoTracking().Include(x => x.Member).ThenInclude(x => x.User).Include(x => x.Trainer).ThenInclude(x => x!.User).Include(x => x.Meals).Where(x => x.IsActive);
        if (User.IsInRole("Member")) q = q.Where(x => x.Member.UserId == user.Id);
        else if (User.IsInRole("Trainer")) q = q.Where(x => x.Trainer != null && x.Trainer.UserId == user.Id);
        var plans = await q.OrderByDescending(x => x.StartDate).Take(200).ToListAsync();
        if (User.IsInRole("Admin") || User.IsInRole("Reception") || User.IsInRole("Trainer"))
            ViewBag.Members = await _db.Members.AsNoTracking().Include(m => m.User).Where(m => m.IsActive).OrderBy(m => m.User.LastName).Take(500).ToListAsync();
        return View(plans);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> Create(DietPlanCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _users.GetUserAsync(User); if (user == null) return Challenge();
        var member = await _db.Members.Include(m => m.AssignedTrainer).FirstOrDefaultAsync(m => m.Id == model.MemberId && m.IsActive);
        if (member == null) return NotFound(new { message = "Member not found." });
        Trainer? trainer = null;
        if (User.IsInRole("Trainer")) trainer = await _db.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id && t.IsActive);
        else if (model.TrainerId.HasValue) trainer = await _db.Trainers.FirstOrDefaultAsync(t => t.Id == model.TrainerId.Value && t.IsActive);
        else trainer = member.AssignedTrainer;
        if (trainer == null || (User.IsInRole("Trainer") && member.AssignedTrainerId != trainer.Id)) return Forbid();
        if (model.EndDate.HasValue && model.EndDate.Value.Date < model.StartDate.Date) return BadRequest(new { message = "End date cannot be before start date." });
        await _db.DietPlans.Where(x => x.MemberId == model.MemberId && x.IsActive).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        _db.DietPlans.Add(new DietPlan { MemberId = member.Id, TrainerId = trainer.Id, Name = model.Name.Trim(), Goal = model.Goal.Trim(), DailyCalories = model.DailyCalories, ProteinGrams = model.ProteinGrams, CarbsGrams = model.CarbsGrams, FatGrams = model.FatGrams, StartDate = model.StartDate.Date, EndDate = model.EndDate?.Date });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> AddMeal(DietMealCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _users.GetUserAsync(User); if (user == null) return Challenge();
        var plan = await _db.DietPlans.Include(x => x.Trainer).FirstOrDefaultAsync(x => x.Id == model.DietPlanId && x.IsActive);
        if (plan == null) return NotFound();
        if (User.IsInRole("Trainer") && plan.Trainer?.UserId != user.Id) return Forbid();
        _db.DietMeals.Add(new DietMeal { DietPlanId = plan.Id, MealOrder = model.MealOrder, MealType = model.MealType.Trim(), FoodItems = model.FoodItems.Trim(), Notes = model.Notes?.Trim(), Calories = model.Calories });
        await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _users.GetUserAsync(User); if (user == null) return Challenge();
        var plan = await _db.DietPlans.Include(x => x.Trainer).FirstOrDefaultAsync(x => x.Id == id);
        if (plan == null) return NotFound(); if (User.IsInRole("Trainer") && plan.Trainer?.UserId != user.Id) return Forbid();
        plan.IsActive = false; await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
}
