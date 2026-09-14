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
        public async Task<IActionResult> Details(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();
            return View(member);
        }

        [HttpGet]
        public  IActionResult Create()
        {
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

                await _userManager.AddToRoleAsync(user, "Member");

                // Create member profile
                var member = new GymFit.Domain.Entities.Member
                {
                    UserId = user.Id,
                    Address = model.Address,
                    EmergencyContact = model.EmergencyContact,
                    MedicalConditions = model.MedicalConditions,
                    AssignedTrainerId = model.AssignedTrainerId,
                    PrimaryBranchId = await _context.Branches.Where(b => b.IsActive).Select(b => b.Id).FirstOrDefaultAsync(),
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Member created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] MemberViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

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

            return View(new MemberViewModel
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                Address = member.Address,
                EmergencyContact = member.EmergencyContact,
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
