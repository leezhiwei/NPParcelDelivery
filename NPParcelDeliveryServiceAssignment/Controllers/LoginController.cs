using DeepEqual.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;
using System.Buffers.Text;
using System.Data.SqlTypes;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using static System.Net.Mime.MediaTypeNames;

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
            if (HttpContext.Session.GetString("UserID") is not null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Member member)
        {
            var props = typeof(Member).GetProperties(); // getallprop from typeofmember
            foreach (var prop in props)
            { //foreach prop
                object value = prop.GetValue(member, null); // get the value
                if (value is null)
                { // if null
                    ViewData["ErrorMsg"] = "Please fill in all required fields"; // error
                    return View(); // return view
                }
            }
            if (md.CheckEmail(member.EmailAddr))
            {
                ViewData["ErrorMsg"] = "Error: Record exists in Database.";
                return View();
            }
            md.AddMember(member);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> GetSalt()
        {
            string salt = "ThisIsASaltToMakePassHashesNotSoEasyToCrack";
            var plainbytes = System.Text.Encoding.UTF8.GetBytes(salt);
            Salt s = new Salt
            {
                Id = 1,
                SaltString = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Convert.ToString(System.Convert.ToBase64String(plainbytes))))
            };

            return Json(s);
        }
    }
}