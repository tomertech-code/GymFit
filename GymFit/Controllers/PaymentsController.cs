using GymFit.Domain.Entities;
using GymFit.Domain.Enums;
using GymFit.Infrastructure.Data;
using GymFit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Web.Controllers
{

    [Authorize(Roles = "Admin,Reception")]
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ApplicationDbContext _context;

        public PaymentsController(PaymentService paymentService, ApplicationDbContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments
                .Include(p => p.Member)
                    .ThenInclude(m => m.User)
                .Include(p => p.Subscription!)
                    .ThenInclude(s => s.MembershipPlan)
                .OrderByDescending(p => p.PaymentDate)
                .Take(100)
                .Select(p => new
                {
                    p.Id,
                    TransactionId = p.TransactionId ?? "TXN" + p.Id.ToString("D6"),
                    MemberName = p.Member.User.FirstName + " " + p.Member.User.LastName,
                    p.Amount,
                    PaymentDate = p.PaymentDate.ToString("dd MMM yyyy"),
                    p.PaymentMethod,
                    Plan = p.Subscription != null ? p.Subscription.MembershipPlan.Name : "N/A",
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            return Json(payments);
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var monthlyRevenue = await _paymentService.GetMonthlyRevenueAsync();
            var todayStart = DateTime.UtcNow.Date;
            var tomorrowStart = todayStart.AddDays(1);
            var todayPayments = await _context.Payments
                .Where(p => p.PaymentDate >= todayStart && p.PaymentDate < tomorrowStart && p.Status == PaymentStatus.Completed)
                .CountAsync();

            return Json(new
            {
                monthlyRevenue,
                todayRevenue = await _context.Payments
                    .Where(p => p.PaymentDate >= todayStart && p.PaymentDate < tomorrowStart && p.Status == PaymentStatus.Completed)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0m,
                todayPayments,
                pendingPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Pending)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _context.Members
                .Include(m => m.User)
                .Where(m => m.IsActive)
                .Select(m => new
                {
                    m.Id,
                    Name = m.User.FirstName + " " + m.User.LastName
                })
                .ToListAsync();

            return Json(members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Record([FromForm] int memberId, [FromForm] decimal amount,
            [FromForm] string paymentMethod, [FromForm] string? notes, [FromForm] string? transactionId)
        {
            try
            {
                if (memberId <= 0 || amount <= 0 || amount > 100000000m || string.IsNullOrWhiteSpace(paymentMethod))
                    return BadRequest(new { success = false, message = "Please provide a valid member, amount and payment method." });
                if (!await _context.Members.AsNoTracking().AnyAsync(m => m.Id == memberId && m.IsActive))
                    return BadRequest(new { success = false, message = "Selected member was not found or is inactive." });
                var allowedMethods = new[] { "Cash", "Credit Card", "Debit Card", "UPI", "Bank Transfer" };
                if (!allowedMethods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase))
                    return BadRequest(new { success = false, message = "Invalid payment method." });
                transactionId = transactionId?.Trim();
                if (string.IsNullOrWhiteSpace(transactionId))
                    transactionId = "TXN-" + Guid.NewGuid().ToString("N");
                if (transactionId.Length > 64)
                    return BadRequest(new { success = false, message = "Invalid transaction identifier." });

                await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                var existingPayment = await _context.Payments.AsNoTracking()
                    .Where(p => p.TransactionId == transactionId)
                    .Select(p => new { p.Id, p.MemberId, p.Amount })
                    .FirstOrDefaultAsync();
                if (existingPayment is not null)
                {
                    if (existingPayment.MemberId == memberId && existingPayment.Amount == amount)
                        return Json(new { success = true, message = "Payment was already recorded." });
                    return Conflict(new { success = false, message = "This transaction identifier is already in use." });
                }

                var member = await _context.Members
                    .AsNoTracking()
                    .Where(m => m.Id == memberId && m.IsActive)
                    .Select(m => new { m.Id, m.PrimaryBranchId })
                    .FirstOrDefaultAsync();
                if (member is null || member.PrimaryBranchId <= 0)
                    return BadRequest(new { success = false, message = "Selected member has no valid primary branch." });

                var activeSubscription = await _context.Subscriptions
                    .AsNoTracking()
                    .Where(s => s.MemberId == memberId && s.IsActive && s.Status == SubscriptionStatus.Active)
                    .OrderByDescending(s => s.EndDate)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                var branchId = member.PrimaryBranchId;
                var payment = new Payment
                {
                    MemberId = memberId,
                    BranchId = branchId,
                    SubscriptionId = activeSubscription == 0 ? null : activeSubscription,
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = paymentMethod,
                    Status = PaymentStatus.Completed,
                    TransactionId = transactionId,
                    Notes = notes
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Payment recorded successfully" });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { success = false, message = "This payment could not be recorded because the transaction was already processed or the data changed. Please refresh and try again." });
            }
        }
    }
}
