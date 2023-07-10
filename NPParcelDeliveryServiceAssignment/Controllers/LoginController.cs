using DeepEqual.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;
using System.Runtime.InteropServices.ObjectiveC;

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

        public ActionResult Register() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Member member)
        {
            var props = typeof(Member).GetProperties();
            foreach (var prop in props)
            {
                object value = prop.GetValue(member, null);
                if (value is null)
                {
                    ViewData["ErrorMsg"] = "Please fill in all required fields";
                    return View();
                }
            }
            List<Member> mlist = md.GetAllMember();
            foreach (Member m in mlist)
            {
                if (m.EmailAddr == member.EmailAddr)
                {
                    ViewData["ErrorMsg"] = "Error: Record exists in Database.";
                    return View();
                }
            }
			md.AddMember(member);
            return RedirectToAction("Index");
		}
	}
}