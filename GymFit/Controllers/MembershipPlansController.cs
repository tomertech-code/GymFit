using GymFit.Domain.Entities;
using GymFit.Domain.Enums;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers
{

    [Authorize(Roles = "Admin,Reception")]
    public class MembershipPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _context.MembershipPlans
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.DurationDays,
                    p.Type,
                    p.Features,
                    p.IsActive,
                    SubscriberCount = p.Subscriptions.Count(s => s.IsActive)
                })
                .ToListAsync();

            return Json(plans);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] string name, [FromForm] string description,
            [FromForm] decimal price, [FromForm] int durationDays, [FromForm] int type, [FromForm] string features)
        {
            try
            {
                var plan = new MembershipPlan
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    DurationDays = durationDays,
                    Type = (MembershipType)type,
                    Features = features,
                    IsActive = true
                };

                _context.MembershipPlans.Add(plan);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Plan created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var plan = await _context.MembershipPlans.FindAsync(id);
                if (plan == null)
                    return Json(new { success = false, message = "Plan not found" });

                plan.IsActive = false;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Plan deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}
