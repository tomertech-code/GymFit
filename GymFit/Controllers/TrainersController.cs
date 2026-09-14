using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View("~/Views/Trainer/Dashboard.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.User)
                .Include(t => t.AssignedMembers)
                .Include(t => t.PrimaryBranch)
                .Select(t => new TrainerViewModel
                {
                    Id = t.Id,
                    Name = t.User.FirstName + " " + t.User.LastName,
                    Email = t.User.Email ?? string.Empty,
                    PhoneNumber = t.User.PhoneNumber ?? string.Empty,
                    Specialization = t.Specialization,
                    ExperienceYears = t.ExperienceYears,
                    MemberCount = t.AssignedMembers.Count,
                    IsActive = t.IsActive,
                    BranchName = t.PrimaryBranch.Name
                })
                .ToListAsync();

            return View(trainers);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllTrainers()
        //{
        //    var trainers = await _context.Trainers
        //        .Include(t => t.User)
        //        .Where(t => t.IsActive)
        //        .Select(t => new
        //        {
        //            t.Id,
        //            Name = t.User.FirstName + " " + t.User.LastName,
        //            t.User.Email,
        //            t.User.PhoneNumber,
        //            t.Specialization,
        //            t.ExperienceYears,
        //            t.Certifications,
        //            MemberCount = t.AssignedMembers.Count,
        //            t.IsActive
        //        })
        //        .ToListAsync();

        //    return Json(trainers);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] string firstName, [FromForm] string lastName,
            [FromForm] string email, [FromForm] string phoneNumber, [FromForm] string specialization,
            [FromForm] int experienceYears, [FromForm] string certifications, [FromForm] string bio, [FromForm] string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });

                await _userManager.AddToRoleAsync(user, "Trainer");

                var trainer = new Trainer
                {
                    UserId = user.Id,
                    Specialization = specialization,
                    ExperienceYears = experienceYears,
                    Certifications = certifications,
                    Bio = bio,
                    PrimaryBranchId = await _context.Branches.Where(b => b.IsActive).Select(b => b.Id).FirstOrDefaultAsync(),
                    IsActive = true
                };

                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Trainer created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var trainer = await _context.Trainers.FindAsync(id);
                if (trainer == null)
                    return Json(new { success = false, message = "Trainer not found" });

                trainer.IsActive = false;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Trainer deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}
