using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymFit.Infrastructure.Data;

namespace GymFit.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid login data" });

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null || !user.IsActive)
                {
                    await _signInManager.SignOutAsync();
                    return Json(new { success = false, message = "This account is inactive." });
                }
                var roles = await _userManager.GetRolesAsync(user!);

                var redirectUrl = roles.Contains("Admin") ? "/AdminDashboard/Index" :
                                roles.Contains("Reception") ? "/ReceptionDashboard/Index" :
                                roles.Contains("Trainer") ? "/Trainer/Dashboard" :
                                roles.Contains("Member") ? "/Member/Dashboard" : "/";

                return Json(new { success = true, redirectUrl });
            }

            return Json(new { success = false, message = "Invalid email or password" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid registration data" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");

                var member = new GymFit.Domain.Entities.Member
                {
                    UserId = user.Id,
                    Address = model.Address,
                    EmergencyContact = model.EmergencyContact,
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };

                var branch = await _context.Branches.FirstOrDefaultAsync(b => b.IsActive);
                if (branch is null)
                {
                    await _userManager.DeleteAsync(user);
                    return Json(new { success = false, message = "Registration is temporarily unavailable. Please try again later." });
                }

                member.PrimaryBranchId = branch.Id;
                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);

                return Json(new { success = true, redirectUrl = "/Member/Dashboard" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Json(new { success = false, message = errors });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
