using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class FeedbackController : Controller
    {
        private MemberDAL mdal = new MemberDAL();
        private FeedbackDAL fdal = new FeedbackDAL();
        private StaffDAL sdal = new StaffDAL();
        public IActionResult MakeFeedback()
        {
            return View();
        }
        [HttpPost]
        public IActionResult MakeFeedback(FeedbackEnquiry f)
        {
            Member m = mdal.GetMemberfromLoginID(HttpContext.Session.GetString("UserID"));
            f.MemberID = m.MemberID;
            f.DateTimePosted = DateTime.Now;
            f.Status = "0";
            fdal.Add(f);
            ViewData["Success"] = "You have successfully written to the database.";
            return View();
        }
        public IActionResult Index()
        {
            Member m = mdal.GetMemberfromLoginID(HttpContext.Session.GetString("UserID"));
            List<FeedbackEnquiry> l = fdal.GetMemberFeedback(m);
            return View(l);
        }
        public IActionResult FeedbackReview()
        {
            List<FeedbackEnquiry> l = fdal.GetAllFeedback();
            return View(l);
        }
        public IActionResult Respond(int? id)
        {
            int idd = 0;
            try
            {
                idd = Convert.ToInt32(id);
            }
            catch
            {
                ViewData["Error"] = "Converting error.";
                return View();
            }
            FeedbackEnquiry f = fdal.GetOneFeedback(idd);
            TempData["Object"] = JsonConvert.SerializeObject(f);
            return View(f);
        }
        [HttpPost]
        public IActionResult Respond(FeedbackEnquiry f)
        {
            FeedbackEnquiry fe = JsonConvert.DeserializeObject<FeedbackEnquiry>((string)TempData["Object"]);
            fe.Response = f.Response;
            fe.Status = "1";
            fe.StaffID = sdal.ReturnStaffID(HttpContext.Session.GetString("UserID"));
            fdal.Update(fe);
            ViewData["Success"] = "You have successfully updated the database.";
            return View();
        }
    }
}
