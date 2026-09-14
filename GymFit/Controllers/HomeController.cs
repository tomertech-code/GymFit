using System.Diagnostics;
using GymFit.Models;
using Microsoft.AspNetCore.Mvc;
using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Plans()
        {
            var plans = await _context.MembershipPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
            return View(plans);
        }

        public async Task<IActionResult> Trainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .ToListAsync();
            return View(trainers);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid form data" });

            try
            {
                var message = new ContactMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Subject = model.Subject,
                    Message = model.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thank you! We'll get back to you soon." });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred. Please try again." });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }

}
