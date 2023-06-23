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
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index","Login");
            }
            string uid = HttpContext.Session.GetString("UserID");
            string tou = HttpContext.Session.GetString("TypeOfUser");
            ViewData["uid"] = uid;
            ViewData["typeofuser"] = tou;
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}