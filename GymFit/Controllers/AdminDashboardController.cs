using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymFit.Services;

namespace GymFit.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    //[Area("Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public AdminDashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Json(stats);
        }
    }
}
