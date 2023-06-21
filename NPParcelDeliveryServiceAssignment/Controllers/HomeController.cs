using Microsoft.AspNetCore.Mvc;
using NPParcelDeliveryServiceAssignment.Models;
using System.Diagnostics;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string uid = HttpContext.Session.GetString("UserID");
            string role = HttpContext.Session.GetString("Role");
            ViewData["uid"] = uid;
            ViewData["Role"] = role;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}