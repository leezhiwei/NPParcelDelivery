using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class LoginController : Controller
    {
        private StaffDAL sd = new StaffDAL();
        private MemberDAL md = new MemberDAL();
        // GET: LoginController
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("UserID") is not null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(IFormCollection form)
        {
            bool isMember = false;
            string userID = form["UserID"].ToString();
            string password = form["UserPass"].ToString();
            if (userID.Contains("@"))
            {
                isMember = true;
            }
            if (!isMember)
            {
                Staff s = sd.CheckStaff(userID, password);
                if (s is not null)
                {
                    HttpContext.Session.SetString("UserID", userID);
                    HttpContext.Session.SetString("TypeOfUser", s.Appointment);
                    return RedirectToAction("Index", "Home");
                }
                ViewData["ErrorMsg"] = "Invalid Username or Password";
                return View();
            }
            Member m = md.CheckMember(userID, password);
            if (m is not null)
            {
                HttpContext.Session.SetString("UserID", userID);
                HttpContext.Session.SetString("TypeOfUser", "Member");
                return RedirectToAction("Index", "Home");
            }
            ViewData["ErrorMsg"] = "Invalid Username or Password";
            return View();
        }

        public ActionResult Register() {
            return View();
        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Member member)
        {

        }*/
    }
}