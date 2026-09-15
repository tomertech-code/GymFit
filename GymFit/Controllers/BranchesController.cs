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
                .OrderBy(b => b.Name)
                .Take(200)
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
        public async Task<IActionResult> Create([Bind("Name,Code,Address,City,State,PinCode,PhoneNumber,Email,ManagerName,ManagerPhone,OpeningTime,ClosingTime")] Branch branch)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(branch.Name) || string.IsNullOrWhiteSpace(branch.Code))
                    return BadRequest(new { success = false, message = "Branch name and code are required." });
                branch.Name = branch.Name.Trim();
                branch.Code = branch.Code.Trim().ToUpperInvariant();
                branch.IsActive = true;
                branch.CreatedAt = DateTime.UtcNow;
                if (branch.OpeningTime >= branch.ClosingTime)
                    return BadRequest(new { success = false, message = "Opening time must be before closing time." });
                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Branch created successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Unable to create the branch right now. Please check the details and try again." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return branch is null ? NotFound() : View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Address,City,State,PinCode,PhoneNumber,Email,ManagerName,ManagerPhone,OpeningTime,ClosingTime,IsActive")] Branch model)
        {
            if (id != model.Id) return BadRequest();
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Code))
                ModelState.AddModelError(string.Empty, "Branch name and code are required.");
            if (model.OpeningTime >= model.ClosingTime)
                ModelState.AddModelError(string.Empty, "Opening time must be before closing time.");
            if (!ModelState.IsValid) return View(model);

            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch is null) return NotFound();
            branch.Name = model.Name.Trim();
            branch.Code = model.Code.Trim().ToUpperInvariant();
            branch.Address = model.Address?.Trim() ?? string.Empty;
            branch.City = model.City?.Trim() ?? string.Empty;
            branch.State = model.State?.Trim() ?? string.Empty;
            branch.PinCode = model.PinCode?.Trim() ?? string.Empty;
            branch.PhoneNumber = model.PhoneNumber?.Trim() ?? string.Empty;
            branch.Email = model.Email?.Trim() ?? string.Empty;
            branch.ManagerName = model.ManagerName?.Trim();
            branch.ManagerPhone = model.ManagerPhone?.Trim();
            branch.OpeningTime = model.OpeningTime;
            branch.ClosingTime = model.ClosingTime;
            branch.IsActive = model.IsActive;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException) { ModelState.AddModelError(nameof(model.Code), "Branch code already exists."); return View(model); }
            TempData["Success"] = "Branch updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return branch is null ? NotFound() : Json(new
            {
                branch.Id, branch.Name, branch.Code, branch.Address, branch.City, branch.State,
                branch.PinCode, branch.PhoneNumber, branch.Email, branch.ManagerName, branch.ManagerPhone,
                OpeningTime = branch.OpeningTime.ToString(@"hh\:mm"),
                ClosingTime = branch.ClosingTime.ToString(@"hh\:mm"), branch.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch is null) return NotFound(new { success = false, message = "Branch not found." });
            if (await _context.Members.AnyAsync(m => m.PrimaryBranchId == id && m.IsActive) || await _context.Trainers.AnyAsync(t => t.PrimaryBranchId == id && t.IsActive))
                return Conflict(new { success = false, message = "This branch has active members or trainers and cannot be deleted." });
            branch.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Branch deactivated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberAccess(int memberId)
        {
            if (memberId <= 0) return BadRequest();
            var rows = await _context.MemberBranchAccesses.AsNoTracking()
                .Where(x => x.MemberId == memberId)
                .OrderBy(x => x.Branch.Name)
                .Select(x => new
                {
                    x.Id, x.MemberId, x.BranchId, BranchName = x.Branch.Name, BranchCode = x.Branch.Code,
                    x.HasAccess, x.GrantedDate, x.ExpiryDate, x.Notes
                }).ToListAsync();
            return Json(rows);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainerBranchAssignments(int trainerId)
        {
            if (trainerId <= 0) return BadRequest();
            var rows = await _context.TrainerBranchAssignments.AsNoTracking()
                .Where(x => x.TrainerId == trainerId)
                .OrderBy(x => x.Branch.Name)
                .Select(x => new
                {
                    x.Id, x.TrainerId, x.BranchId, BranchName = x.Branch.Name, BranchCode = x.Branch.Code,
                    x.WorkingDays, x.StartTime, x.EndTime, x.IsActive
                }).ToListAsync();
            return Json(rows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantMemberAccess(int memberId, int branchId, DateTime? expiryDate, string? notes)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId && m.IsActive);
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.IsActive);
            if (member is null || branch is null)
                return NotFound(new { success = false, message = "Member or branch not found." });

            if (member.PrimaryBranchId == branchId)
                return BadRequest(new { success = false, message = "The member's primary branch already provides access." });

            if (expiryDate.HasValue && expiryDate.Value.Date < DateTime.UtcNow.Date)
                return BadRequest(new { success = false, message = "Expiry date cannot be in the past." });

            var existing = await _context.MemberBranchAccesses
                .FirstOrDefaultAsync(x => x.MemberId == memberId && x.BranchId == branchId);
            if (existing is null)
            {
                _context.MemberBranchAccesses.Add(new MemberBranchAccess
                {
                    MemberId = memberId,
                    BranchId = branchId,
                    HasAccess = true,
                    GrantedDate = DateTime.UtcNow,
                    ExpiryDate = expiryDate?.Date,
                    Notes = notes?.Trim()
                });
            }
            else
            {
                existing.HasAccess = true;
                existing.ExpiryDate = expiryDate?.Date;
                existing.Notes = notes?.Trim();
                existing.GrantedDate = DateTime.UtcNow;
            }

            member.HasMultiBranchAccess = true;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Member branch access granted." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeMemberAccess(int accessId)
        {
            var access = await _context.MemberBranchAccesses
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x => x.Id == accessId);
            if (access is null)
                return NotFound(new { success = false, message = "Branch access not found." });

            access.HasAccess = false;
            access.Member.HasMultiBranchAccess = await _context.MemberBranchAccesses
                .AnyAsync(x => x.MemberId == access.MemberId && x.Id != access.Id && x.HasAccess &&
                               (x.ExpiryDate == null || x.ExpiryDate >= DateTime.UtcNow.Date));
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Member branch access revoked." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTrainerBranch(int trainerId, int branchId, string? workingDays, TimeSpan? startTime, TimeSpan? endTime)
        {
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == trainerId && t.IsActive);
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.IsActive);
            if (trainer is null || branch is null)
                return NotFound(new { success = false, message = "Trainer or branch not found." });

            if (trainer.PrimaryBranchId == branchId)
                return BadRequest(new { success = false, message = "The trainer's primary branch is already assigned." });

            if (startTime.HasValue && endTime.HasValue && startTime.Value >= endTime.Value)
                return BadRequest(new { success = false, message = "Start time must be before end time." });

            var assignment = await _context.TrainerBranchAssignments
                .FirstOrDefaultAsync(x => x.TrainerId == trainerId && x.BranchId == branchId);
            if (assignment is null)
            {
                _context.TrainerBranchAssignments.Add(new TrainerBranchAssignment
                {
                    TrainerId = trainerId,
                    BranchId = branchId,
                    WorkingDays = workingDays?.Trim(),
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = true
                });
            }
            else
            {
                assignment.WorkingDays = workingDays?.Trim();
                assignment.StartTime = startTime;
                assignment.EndTime = endTime;
                assignment.IsActive = true;
            }

            trainer.WorksAtMultipleBranches = true;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Trainer branch assignment saved." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTrainerBranchAssignment(int assignmentId)
        {
            var assignment = await _context.TrainerBranchAssignments
                .Include(x => x.Trainer)
                .FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assignment is null)
                return NotFound(new { success = false, message = "Trainer branch assignment not found." });

            assignment.IsActive = false;
            assignment.Trainer.WorksAtMultipleBranches = await _context.TrainerBranchAssignments
                .AnyAsync(x => x.TrainerId == assignment.TrainerId && x.Id != assignment.Id && x.IsActive);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Trainer branch assignment removed." });
        }

    }

}
