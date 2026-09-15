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
                .OrderBy(t => t.User.LastName)
                .ThenBy(t => t.User.FirstName)
                .Take(500)
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trainers = await _context.Trainers
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.AssignedMembers)
                .Include(t => t.PrimaryBranch)
                .OrderBy(t => t.User.LastName)
                .ThenBy(t => t.User.FirstName)
                .Take(500)
                .Select(t => new
                {
                    t.Id,
                    Name = t.User.FirstName + " " + t.User.LastName,
                    Email = t.User.Email ?? string.Empty,
                    PhoneNumber = t.User.PhoneNumber ?? string.Empty,
                    t.Specialization,
                    t.ExperienceYears,
                    MemberCount = t.AssignedMembers.Count,
                    t.IsActive,
                    BranchName = t.PrimaryBranch.Name
                })
                .ToListAsync();
            return Json(trainers);
        }

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

                var roleResult = await _userManager.AddToRoleAsync(user, "Trainer");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new { success = false, message = "Unable to assign the Trainer role." });
                }

                var primaryBranchId = await _context.Branches.Where(b => b.IsActive).Select(b => b.Id).FirstOrDefaultAsync();
                if (primaryBranchId <= 0)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Trainer");
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new { success = false, message = "No active branch is configured." });
                }

                var trainer = new Trainer
                {
                    UserId = user.Id,
                    Specialization = specialization,
                    ExperienceYears = experienceYears,
                    Certifications = certifications,
                    Bio = bio,
                    PrimaryBranchId = primaryBranchId,
                    IsActive = true
                };

                _context.Trainers.Add(trainer);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    await _userManager.RemoveFromRoleAsync(user, "Trainer");
                    await _userManager.DeleteAsync(user);
                    throw;
                }

                return Json(new { success = true, message = "Trainer created successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Unable to create the trainer right now. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _context.Trainers
                .AsNoTracking()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer is null)
                return NotFound();

            ViewBag.Branches = await _context.Branches.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Specialization,Certifications,ExperienceYears,Bio,PrimaryBranchId,IsActive,WorksAtMultipleBranches")] Trainer model)
        {
            if (id != model.Id)
                return BadRequest();

            if (model.ExperienceYears < 0 || model.ExperienceYears > 60)
                ModelState.AddModelError(nameof(model.ExperienceYears), "Experience must be between 0 and 60 years.");

            if (!await _context.Branches.AsNoTracking().AnyAsync(b => b.Id == model.PrimaryBranchId && b.IsActive))
                ModelState.AddModelError(nameof(model.PrimaryBranchId), "Please select an active branch.");

            if (!ModelState.IsValid)
                return View(model);

            var trainer = await _context.Trainers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer is null)
                return NotFound();

            trainer.Specialization = model.Specialization.Trim();
            trainer.Certifications = model.Certifications?.Trim() ?? string.Empty;
            trainer.ExperienceYears = model.ExperienceYears;
            trainer.Bio = model.Bio?.Trim();
            trainer.PrimaryBranchId = model.PrimaryBranchId;
            trainer.WorksAtMultipleBranches = model.WorksAtMultipleBranches;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Trainer updated successfully.";
            return RedirectToAction(nameof(GetAllTrainers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == id);
            if (trainer is null)
                return NotFound(new { success = false, message = "Trainer not found." });

            trainer.IsActive = !trainer.IsActive;

            var user = await _userManager.FindByIdAsync(trainer.UserId);
            if (user is not null)
                user.IsActive = trainer.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllTrainers));
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
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Unable to complete the operation right now. Please try again." });
            }
        }
    }

}
