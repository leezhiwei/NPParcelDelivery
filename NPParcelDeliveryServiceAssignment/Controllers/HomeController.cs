using Microsoft.AspNetCore.Mvc;
using NPParcelDeliveryServiceAssignment.DALs;
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
            StaffDAL sd = new StaffDAL();
            List<Staff> ls = sd.GetAllStaff();
            ViewData["StaffTest"] = ls;
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