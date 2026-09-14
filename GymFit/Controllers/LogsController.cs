using GymFit.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymFit.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LogsController : Controller
    {
        private readonly ILoggingService _loggingService;

        public LogsController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(int count = 100)
        {
            var logs = await _loggingService.GetRecentLogsAsync(count);
            return Json(logs);
        }

        [HttpPost]
        public async Task<IActionResult> ClearOldLogs(int days = 30)
        {
            await _loggingService.ClearOldLogsAsync(days);
            await _loggingService.LogInfoAsync($"Old logs cleared (older than {days} days)");
            return Json(new { success = true, message = $"Logs older than {days} days have been cleared" });
        }

        [HttpPost]
        public async Task<IActionResult> TestError()
        {
            try
            {
                throw new Exception("This is a test error for logging purposes");
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(ex, "Test error triggered by admin");
                return Json(new { success = true, message = "Test error logged successfully" });
            }
        }
    }

}
