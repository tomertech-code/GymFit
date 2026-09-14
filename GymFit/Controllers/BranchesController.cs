using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers
{


    [Authorize(Roles = "Admin")]
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
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
            var branches = await _context.Branches
                .Include(b => b.Members)
                .Include(b => b.Trainers)
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Code,
                    b.Address,
                    b.City,
                    b.State,
                    b.PinCode,
                    b.PhoneNumber,
                    b.Email,
                    b.ManagerName,
                    b.ManagerPhone,
                    OpeningTime = b.OpeningTime.ToString(@"hh\:mm"),
                    ClosingTime = b.ClosingTime.ToString(@"hh\:mm"),
                    b.IsActive,
                    MemberCount = b.Members.Count,
                    TrainerCount = b.Trainers.Count
                })
                .ToListAsync();

            return Json(branches);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Branch branch)
        {
            try
            {
                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Branch created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}
