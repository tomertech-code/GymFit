using GymFit.Application.Interfaces;
using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers
{


    [Authorize(Roles = "Admin,Reception")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembersController(IMemberService memberService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _memberService = memberService;
            _context = context;
            _userManager = userManager;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
         {
            var members = await _memberService.GetAllMembersAsync();
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _context.Members.AsNoTracking()
                .Include(m => m.User).Where(m => m.IsActive)
                .OrderBy(m => m.User.LastName).ThenBy(m => m.User.FirstName)
                .Take(500)
                .Select(m => new { m.Id, m.User.FirstName, m.User.LastName, Email = m.User.Email ?? string.Empty, PhoneNumber = m.User.PhoneNumber ?? string.Empty, m.JoinDate, m.IsActive })
                .ToListAsync();
            return Json(members);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _context.Branches.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] MemberViewModel model, [FromForm] string password)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            try
            {
                // Create user account
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = DateTime.Now.AddYears(-20),
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });

                var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new { success = false, message = "Unable to assign the Member role." });
                }

                // Create member profile
                var member = new GymFit.Domain.Entities.Member
                {
                    UserId = user.Id,
                    Address = model.Address,
                    EmergencyContact = model.EmergencyContact,
                    MedicalConditions = model.MedicalConditions,
                    AssignedTrainerId = model.AssignedTrainerId,
                    PrimaryBranchId = model.PrimaryBranchId,
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };

                if (member.PrimaryBranchId <= 0 || !await _context.Branches.AnyAsync(b => b.Id == member.PrimaryBranchId && b.IsActive))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Member");
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new { success = false, message = "Please select an active primary branch." });
                }

                if (member.AssignedTrainerId.HasValue && !await _context.Trainers.AnyAsync(t => t.Id == member.AssignedTrainerId.Value && t.IsActive))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Member");
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new { success = false, message = "Selected trainer is not active." });
                }

                _context.Members.Add(member);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    await _userManager.RemoveFromRoleAsync(user, "Member");
                    await _userManager.DeleteAsync(user);
                    throw;
                }

                return Json(new { success = true, message = "Member created successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Unable to create the member right now. Please try again." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] MemberViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            if (!await _context.Branches.AnyAsync(b => b.Id == model.PrimaryBranchId && b.IsActive))
                return BadRequest(new { success = false, message = "Please select an active primary branch." });

            if (model.AssignedTrainerId.HasValue && !await _context.Trainers.AnyAsync(t => t.Id == model.AssignedTrainerId.Value && t.IsActive))
                return BadRequest(new { success = false, message = "Selected trainer is not active." });

            var result = await _memberService.UpdateMemberAsync(model);
            if (result)
                return Json(new { success = true, message = "Member updated successfully" });

            return Json(new { success = false, message = "Failed to update member" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member is null)
                return NotFound();

            ViewBag.Branches = await _context.Branches.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View(new MemberViewModel
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                Address = member.Address,
                EmergencyContact = member.EmergencyContact,
                PrimaryBranchId = member.PrimaryBranchId,
                AssignedTrainerId = member.AssignedTrainerId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _memberService.DeleteMemberAsync(id);
            if (result)
                return Json(new { success = true, message = "Member deleted successfully" });

            return Json(new { success = false, message = "Failed to delete member" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, bool confirm = true)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            return member is null ? NotFound() : View(member);
        }
    }

}
