using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GymFit.Web.Controllers;
[Authorize(Roles="Admin,Reception,Trainer,Member")]
public class MeasurementsController:Controller{
 private readonly ApplicationDbContext _db;private readonly UserManager<ApplicationUser> _users;public MeasurementsController(ApplicationDbContext db,UserManager<ApplicationUser> users){_db=db;_users=users;}
    public async Task<IActionResult> Index(int? memberId)
    {
        var u = await _users.GetUserAsync(User);

        if (u == null)
            return Challenge();

        var q = _db.BodyMeasurements
            .AsNoTracking()
            .Include(x => x.Member)
            .ThenInclude(m => m.User)
            .OrderByDescending(x => x.MeasurementDate)
            .AsQueryable();

        if (User.IsInRole("Member"))
        {
            q = q.Where(x => x.Member.UserId == u.Id);
        }
        else if (memberId.HasValue)
        {
            q = q.Where(x => x.MemberId == memberId.Value);
        }

        ViewBag.MemberId = memberId;

        if (User.IsInRole("Member"))
        {
            ViewBag.Members = Array.Empty<GymFit.Domain.Entities.Member>();
        }
        else
        {
            ViewBag.Members = await _db.Members
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.IsActive)
                .OrderBy(m => m.User.LastName)
                .Take(500)
                .ToListAsync();
        }

        return View(await q.Take(300).ToListAsync());
    }
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles="Admin,Reception,Trainer")]
 public async Task<IActionResult>Create(BodyMeasurementViewModel m){if(!ModelState.IsValid)return BadRequest(ModelState);var member=await _db.Members.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==m.MemberId&&x.IsActive);if(member==null)return NotFound();var user=await _users.GetUserAsync(User);if(user==null)return Challenge();if(User.IsInRole("Trainer")){var trainer=await _db.Trainers.AsNoTracking().FirstOrDefaultAsync(t=>t.UserId==user.Id&&t.IsActive);if(trainer==null||member.AssignedTrainerId!=trainer.Id)return Forbid();}_db.BodyMeasurements.Add(new BodyMeasurement{MemberId=m.MemberId,MeasurementDate=m.MeasurementDate.Date,ChestCm=m.ChestCm,WaistCm=m.WaistCm,HipsCm=m.HipsCm,LeftArmCm=m.LeftArmCm,RightArmCm=m.RightArmCm,LeftThighCm=m.LeftThighCm,RightThighCm=m.RightThighCm,Notes=m.Notes?.Trim()});await _db.SaveChangesAsync();return RedirectToAction(nameof(Index),new{memberId=m.MemberId});}
 [HttpPost,ValidateAntiForgeryToken,Authorize(Roles="Admin,Reception,Trainer")]
 public async Task<IActionResult>Delete(int id){var x=await _db.BodyMeasurements.FindAsync(id);if(x==null)return NotFound();_db.BodyMeasurements.Remove(x);await _db.SaveChangesAsync();return RedirectToAction(nameof(Index),new{memberId=x.MemberId});}
}
