using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymFit.Infrastructure.Data;

namespace GymFit.Web.Controllers.Member
{

    [Authorize(Roles = "Member")]
    [Route("Member")]
    public class DashboardController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _memberService = memberService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var member = await _memberService.GetMemberByUserIdAsync(user!.Id);
            return member is null ? NotFound() : View("~/Views/Members/Dashboard/Index.cshtml", member);
        }

        [HttpGet("Dashboard/Profile")]
        public async Task<IActionResult> Profile()
        {
            var member = await _memberService.GetMemberByUserIdAsync((await _userManager.GetUserAsync(User))!.Id);
            return member is null ? NotFound() : View("~/Views/Members/Dashboard/Profile.cshtml", member);
        }

        [HttpGet("Dashboard/Workouts")]
        public async Task<IActionResult> Workouts()
        {
            var user = await _userManager.GetUserAsync(User);
            var member = await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == user!.Id);
            if (member is null)
                return NotFound();

            var plans = await _context.WorkoutPlans.AsNoTracking()
                .Include(p => p.Trainer).ThenInclude(t => t.User)
                .Include(p => p.Exercises)
                .Where(p => p.MemberId == member.Id && p.IsActive)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
            return View("~/Views/Members/Dashboard/Workouts.cshtml", plans);
        }
    }
}
