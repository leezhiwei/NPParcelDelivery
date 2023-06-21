using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using NPParcelDeliveryServiceAssignment.DALs;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class LoginController : Controller
    {
        private StaffDAL sd = new StaffDAL();
        private MemberDAL md = new MemberDAL();
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(IFormCollection form)
        {
            bool isMember = false;
            string userID = form["UserID"].ToString();
            string password = form["UserPass"].ToString();
            if (userID.Contains("@")){
                isMember = true;
            }
            if (!isMember)
            {
                if (sd.CheckStaff(userID, password))
                {
                    HttpContext.Session.SetString("UserID", userID);
                    HttpContext.Session.SetString("Role", "Staff");
                    return RedirectToAction("Index","Home");
                }
                ViewData["ErrorMsg"] = "Invalid Username or Password";
                return View();
            }
            if (md.CheckMember(userID, password))
            {
                HttpContext.Session.SetString("UserID", userID);
                HttpContext.Session.SetString("Role", "Member");
                return RedirectToAction("Index", "Home");
            }
            ViewData["ErrorMsg"] = "Invalid Username or Password";
            return View();
        }
        
    }
}
