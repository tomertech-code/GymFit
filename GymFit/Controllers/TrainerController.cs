using GymFit.Application.DTOs;
using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Trainer")]
[Route("Trainer")]
public class TrainerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TrainerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var trainer = await GetCurrentTrainerAsync();
        if (trainer is null)
            return Forbid();

        var today = DateTime.UtcNow.Date;
        var members = await _context.Members
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.Subscriptions.Where(s => s.IsActive))
                .ThenInclude(s => s.MembershipPlan)
            .Where(m => m.AssignedTrainerId == trainer.Id && m.IsActive)
            .OrderBy(m => m.User.LastName)
            .Select(m => new MemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                FirstName = m.User.FirstName,
                LastName = m.User.LastName,
                Email = m.User.Email ?? string.Empty,
                PhoneNumber = m.User.PhoneNumber ?? string.Empty,
                Address = m.Address,
                EmergencyContact = m.EmergencyContact,
                CurrentPlan = m.Subscriptions.OrderByDescending(s => s.EndDate).Select(s => s.MembershipPlan.Name).FirstOrDefault(),
                JoinDate = m.JoinDate,
                IsActive = m.IsActive
            })
            .ToListAsync();

        var memberIds = members.Select(m => m.Id).ToArray();
        var model = new TrainerDashboardViewModel
        {
            AssignedMemberCount = members.Count,
            TodayAttendance = await _context.Attendances.CountAsync(a => memberIds.Contains(a.MemberId) && a.CheckInTime >= today),
            AssignedMembers = members
        };

        return View("~/Views/Trainer/DashboardHome.cshtml", model);
    }

    [HttpGet("Members/{id:int}")]
    public async Task<IActionResult> MemberDetails(int id)
    {
        var trainer = await GetCurrentTrainerAsync();
        if (trainer is null)
            return Forbid();

        var member = await _context.Members
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.Subscriptions.Where(s => s.IsActive))
                .ThenInclude(s => s.MembershipPlan)
            .FirstOrDefaultAsync(m => m.Id == id && m.AssignedTrainerId == trainer.Id);

        if (member is null)
            return NotFound();

        return View("~/Views/Trainer/MemberDetails.cshtml", member);
    }

    private async Task<Trainer?> GetCurrentTrainerAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user is null
            ? null
            : await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id && t.IsActive);
    }
}
