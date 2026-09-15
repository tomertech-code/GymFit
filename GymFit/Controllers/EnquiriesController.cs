using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers;

[Authorize(Roles = "Admin,Reception")]
public class EnquiriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public EnquiriesController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var enquiries = await _context.ContactMessages
            .AsNoTracking()
            .OrderByDescending(message => message.CreatedAt)
            .Take(500)
            .ToListAsync();
        return View(enquiries);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var enquiry = await _context.ContactMessages.FindAsync(id);
        if (enquiry is null)
            return NotFound();

        enquiry.IsRead = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var enquiry = await _context.ContactMessages.FindAsync(id);
        if (enquiry is null)
            return NotFound();

        _context.ContactMessages.Remove(enquiry);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
