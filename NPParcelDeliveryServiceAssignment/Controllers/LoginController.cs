using DeepEqual.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json;
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
            if (member.BirthDate > DateTime.Now)
            {
                ViewData["ErrorMsg"] = "Error: Date of Birth cannot be after current Date. Please Re-select Date of Birth";
                return View(member);
            }
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

            if (!member.EmailAddr.Contains("@") || !member.EmailAddr.Contains("."))
            {
                ViewData["ErrorMsg"] = "Error: Invalid email.";
                return View();
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
        public ActionResult UpdateUserDetails()
        {
            UserInfo u = new UserInfo();
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            string tou = HttpContext.Session.GetString("TypeOfUser");
            ViewData["typeofuser"] = tou;
            string LoginID = HttpContext.Session.GetString("UserID");
            if (LoginID.Contains("@"))
            {
                Member m = md.GetMemberfromLoginID(LoginID);
                if (m is null)
                {
                    TempData["Error"] = "An unknown error occured, please contact the developers.";
                    return View();
                }
                u.Id = m.MemberID;
                u.UserType = "Member";
                u.Name = m.Name;
                u.Salutation = m.Salutation;
                u.TelNo = m.TelNo;
                u.Email = m.EmailAddr;
                u.Password = m.Password;
                u.BirthDate = m.BirthDate;
                u.City = m.City;
                u.Country = m.Country;
            }
            else
            {
                Staff s = sd.GetOneStaff(sd.ReturnStaffID(LoginID));
                if (s is null)
                {
                    TempData["Error"] = "An unknown error occured, please contact the developers.";
                    return View();
                }
                u.Id = s.StaffID;
                u.UserType = "Staff";
                u.Name = s.StaffName;
                u.LoginID = s.LoginID;
                u.Password = s.Password;
                u.TelNo = s.OfficeTelNo;
                u.Location = s.Location;
            }
            TempData["obj"] = JsonConvert.SerializeObject(u);
            return View(u);
        }
        [HttpPost]
        public ActionResult UpdateUserDetails(UserInfo u)
        {
            string tou = HttpContext.Session.GetString("TypeOfUser");
            ViewData["typeofuser"] = tou;
            UserInfo us = JsonConvert.DeserializeObject<UserInfo>((string)TempData["obj"]);
            void Merge(UserInfo existingobject, UserInfo somevalues)
            {
                // From stackoverflow, https://stackoverflow.com/questions/8702603/merging-two-objects-in-c-sharp, Reflection method
                Type t = typeof(UserInfo);
                // get type obj of ShippingRate
                var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
                // get property of obj
                foreach (var prop in properties)
                { // foreach property
                    var value = prop.GetValue(somevalues);
                    // get the value from the "blank" from form object, some value null or 0
                    if (value is null)
                    { // if the value is indeed null
                        var valuefromexistingobj = prop.GetValue(existingobject);
                        // get this data from the "existing" object
                        prop.SetValue(somevalues, valuefromexistingobj);
                        // set it into the new object
                        continue; // move on to next property
                    }
                    try // try to convert to int, exception move to catch
                    { // if above never trigger, check for ID 0 or number 0
                        int? numberval = (int)value; // convert to int 
                        if (numberval == 0)
                        { // if the numbervalue is 0, eg id
                            var valuefromexistingobj = prop.GetValue(existingobject);
                            // get this data from "existing" object.
                            prop.SetValue(somevalues, valuefromexistingobj);
                            // set it into new object
                            continue; // move on to next property
                        }
                    }
                    catch // catch the exception
                    {
                        continue; // move to new object
                    }
                }
            }
            Merge(us, u);
            if (u.UserType == "Staff")
            {
                Staff s = sd.GetOneStaff(u.Id);
                s.StaffName = u.Name;
                s.LoginID = u.LoginID;
                s.Password = u.Password;
                s.OfficeTelNo = u.TelNo;
                s.Location = u.Location;
                sd.Update(s);
                ViewData["Success"] = "Database updated successfully";
                TempData["obj"] = JsonConvert.SerializeObject(u);
                return View(u);
            }
            else
            {
                Member m = md.GetMIDByID(u.Id);
                m.Name = u.Name;
                m.Salutation = u.Salutation;
                m.TelNo = u.TelNo;
                m.EmailAddr = u.Email;
                m.Password = u.Password;
                m.BirthDate = u.BirthDate;
                m.City = u.City;
                m.Country = u.Country;
                md.Update(m);
                ViewData["Success"] = "Database updated successfully";
                TempData["obj"] = JsonConvert.SerializeObject(u);
                return View(u);
            }
        }
    }
}