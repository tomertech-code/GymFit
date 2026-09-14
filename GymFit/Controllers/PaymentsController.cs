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
            var todayPayments = await _context.Payments
                .Where(p => p.PaymentDate.Date == DateTime.Today && p.Status == PaymentStatus.Completed)
                .CountAsync();

            return Json(new
            {
                monthlyRevenue,
                todayRevenue = await _context.Payments
                    .Where(p => p.PaymentDate.Date == DateTime.Today && p.Status == PaymentStatus.Completed)
                    .SumAsync(p => p.Amount),
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
            [FromForm] string paymentMethod, [FromForm] string? notes)
        {
            try
            {
                var payment = new Payment
                {
                    MemberId = memberId,
                    BranchId = await _context.Branches.Where(b => b.IsActive).Select(b => b.Id).FirstOrDefaultAsync(),
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = paymentMethod,
                    Status = PaymentStatus.Completed,
                    TransactionId = "TXN" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Notes = notes
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Payment recorded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
