using GymFit.Application.ViewModels;

using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using GymFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Controllers;

[Authorize(Roles = "Admin")]
public class EquipmentController : Controller
{
    private readonly ApplicationDbContext _db;

    public EquipmentController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int? branchId)
    {
        var query = _db.Branches
            .AsNoTracking()
            .Include(branch => branch.Equipment)
            .AsQueryable();

        if (branchId.HasValue)
        {
            query = query.Where(branch => branch.Id == branchId.Value);
        }

        ViewBag.Branches = await _db.Branches
            .AsNoTracking()
            .Where(branch => branch.IsActive)
            .OrderBy(branch => branch.Name)
            .ToListAsync();

        var branches = await query
            .OrderBy(branch => branch.Name)
            .Take(100)
            .ToListAsync();

        return View(branches);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EquipmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(model.EquipmentName) ||
            string.IsNullOrWhiteSpace(model.Category))
        {
            ModelState.AddModelError(
                string.Empty,
                "Equipment name and category are required.");

            return BadRequest(ModelState);
        }

        var branchExists = await _db.Branches.AnyAsync(branch =>
            branch.Id == model.BranchId &&
            branch.IsActive);

        if (!branchExists)
        {
            return NotFound();
        }

        var equipment = new BranchEquipment
        {
            BranchId = model.BranchId,
            EquipmentName = model.EquipmentName.Trim(),
            Category = model.Category.Trim(),
            Quantity = model.Quantity,
            Brand = model.Brand?.Trim(),
            PurchaseDate = model.PurchaseDate?.Date,
            MaintenanceSchedule = model.MaintenanceSchedule?.Trim(),
            IsWorking = model.IsWorking
        };

        _db.BranchEquipment.Add(equipment);
        await _db.SaveChangesAsync();

        return RedirectToAction(
            nameof(Index),
            new { branchId = model.BranchId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var equipment = await _db.BranchEquipment
            .SingleOrDefaultAsync(e => e.Id == id);

        if (equipment is null)
        {
            return NotFound();
        }

        equipment.IsWorking = !equipment.IsWorking;
        await _db.SaveChangesAsync();

        return RedirectToAction(
            nameof(Index),
            new { branchId = equipment.BranchId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var equipment = await _db.BranchEquipment.FindAsync(id);

        if (equipment is null)
        {
            return NotFound();
        }

        var branchId = equipment.BranchId;

        _db.BranchEquipment.Remove(equipment);
        await _db.SaveChangesAsync();

        return RedirectToAction(
            nameof(Index),
            new { branchId });
    }
}
