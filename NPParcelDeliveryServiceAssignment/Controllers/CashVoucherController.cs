using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class CashVoucherController : Controller
    {
        private CashVoucherDAL clist = new CashVoucherDAL();
        private MemberDAL mlist = new MemberDAL();
        private StaffDAL sdal = new StaffDAL();
        private DeliveryFailureDAL dflist = new DeliveryFailureDAL();
        private ParcelDAL plist = new ParcelDAL();
        private DeliveryHistoryDAL dhlist = new DeliveryHistoryDAL();
        public ActionResult FailurereportList()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<DeliveryFailure> dlist = dflist.GetAllFailureReport();
            return View(dlist);
        }

        public ActionResult CashvoucherList()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["showcv"] = false;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashvoucherList(IFormCollection form)
        {
            ViewData["showcv"] = false;
            List<CashVoucher> cv = clist.GetAllCashVoucher();
            string rname = (form["nameBox"]);
            string tnum = (form["telBox"]);
            List<CashVoucher> voucherlist = new List<CashVoucher>();

            CashVoucher gcashvoucher = null;
            if (clist.GetCVIDByNameAndTelNum(rname, tnum) != null)
            {

                TempData["NOResult"] = "";
                ViewData["showcv"] = true;
                gcashvoucher = clist.GetCVIDByNameAndTelNum(rname, tnum);
                if (clist.GetCVIDByNameAndTelNum(rname, tnum).Status == "0")
                {
                    gcashvoucher.Status = "Pending Collection";
                }
                if (clist.GetCVIDByNameAndTelNum(rname, tnum).Status == "1")
                {
                    gcashvoucher.Status = "Collected";
                }
                voucherlist.Add(gcashvoucher);
            }
            else
            { TempData["NOResult"] = "NO Result Found"; }
            
            return View(voucherlist);
        }

        public ActionResult IssueCashVoucherList()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<Member> mblist = mlist.GetMemberDOBMonth();
            return View(mblist);
        }

        public ActionResult CashVoucherUpdate(int id)
        {
            int idd = 0;
            idd = Convert.ToInt32(id);
            List<CashVoucher> cc = clist.GetAllCashVoucher();
            if (clist.GetCVIDByID(id).CashVoucherID == idd)
            {
                return View(clist.GetCVIDByID(id));
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashVoucherUpdate(CashVoucher c)
        {
            if (c.Status == "0")
            {
                c.Status = "1";
                clist.Update(c);
                TempData["SuccessIssued"] = "You have successfully collect your cash voucher";
                //return RedirectToAction("CashvoucherList") ;
            }
            else
            {
                TempData["notIssued"] = "You have already collected your cash voucher, cannot collect again";
            }
            return View();
        }
            // GET: CashVoucherController1/Details/5
            public ActionResult Details(int id)
        {
            return View();
        }

		public ActionResult IssueSpecialVoucher(int id)
		{
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["CanIssue"] = false;
            ViewData["CannotIssue"] = false;
			//----------------
			List<Member>mblist = mlist.GetAllMember();
            Member mm = null;
            if (mlist.GetMIDByID(id).MemberID == id)
            {
                mm = mlist.GetMIDByID(id);
            }
            if (mm is null)
            {
                ViewData["error"] = $"Error";
                return View();
            }

            TempData["Member"] = JsonConvert.SerializeObject(mm);

            DateTime now = DateTime.Now;
            int thismonth = Convert.ToInt32(now.Month.ToString());
            DateTime mbirthDay = (DateTime)mm.BirthDate;
            //string[] birthMonthh = mbirthDay.Split("/");
            int birthMonth =mbirthDay.Month;
            if (thismonth == birthMonth)
            {
                ViewData["CanIssue"] = true;
                ViewData["CannotIssue"] = false;
                TempData["IssueAmount"] = "Issue Amount: $10";
            }
            else
            {
                ViewData["CanIssue"] = false;
                ViewData["CannotIssue"] = true;
            }
			//----------------
			return View();
            
		}

		// POST: CashVoucherController1/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult IssueSpecialVoucher(CashVoucher cashVoucher)
		{
            ViewData["CanIssue"] = true;
            ViewData["CannotIssue"] = false;
            int yearcheck = DateTime.Now.Year;
            Member m = null;
            //----------------
            try
            {
                m = JsonConvert.DeserializeObject<Member>((string)TempData["Member"]);
            }
            catch
            {
                return View();
            }
            string stID = (string)HttpContext.Session.GetString("UserID");
            List<CashVoucher> nclist = clist.GetAllCashVoucher();
            List<Staff> ls = sdal.GetAllStaff();
            int staffIDCheck = sdal.ReturnStaffID(stID);
            if (staffIDCheck <= -1)
            {
                return View();
            }
            else
            { cashVoucher.StaffID = sdal.ReturnStaffID(stID); }
            cashVoucher.Amount = 10;
            cashVoucher.Currency = "SGD";
            cashVoucher.IssuingCode = "1";
            cashVoucher.ReceiverName = m.Name;
            cashVoucher.ReceiverTelNo = m.TelNo;
            cashVoucher.DateTimeIssued = DateTime.Now;
            cashVoucher.Status = "0";
            bool allowAdd =false;
            CashVoucher cvv = clist.GetCVIDByNameAndTelNum2(cashVoucher.ReceiverName, cashVoucher.ReceiverTelNo,1);
            if (cvv.DateTimeIssued.Year == DateTime.Now.Year)
            {
                TempData["readyIssued"] = "Issuse Cash Voucher Failed! You have already Issue cash voucher this year!";
            }
            else 
            {
                cashVoucher.CashVoucherID = clist.Add(cashVoucher);
                TempData["Issued"] = "You have successfully issue cash voucher";
            }
            return View();
		}


        public ActionResult IssueCompensationVoucher(int id)
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            CashVoucher cashVoucher = new CashVoucher();
            //----------------
            List<DeliveryHistory>dhhlist = dhlist.GetAllHistory();
            List<DeliveryFailure> dlist = dflist.GetAllFailureReport();
            DeliveryFailure df = null;
            df = dflist.GetOne(id);
            int yearcheck = DateTime.Now.Year;
            DeliveryFailure d = null;
            string stID = (string)HttpContext.Session.GetString("UserID");
            List<CashVoucher> nclist = clist.GetAllCashVoucher();
            List<Staff> ls = sdal.GetAllStaff();
            int staffIDCheck = sdal.ReturnStaffID(stID);
            if (staffIDCheck <= -1)
            {
                return View();
            }
            else
            { cashVoucher.StaffID = sdal.ReturnStaffID(stID); }
            Parcel par = null;
            List<Parcel> palist = plist.GetAllParcel();
            int pd = df.ParcelID;
            par = plist.GetPIDByPID(pd);
            cashVoucher.ReceiverName = par.ReceiverName;
            cashVoucher.ReceiverTelNo = par.ReceiverTelNo;
            cashVoucher.Amount = 20;
            cashVoucher.Currency = "SGD";
            cashVoucher.IssuingCode = "2";
            cashVoucher.DateTimeIssued = DateTime.Now;
            cashVoucher.Status = "0";
            bool allowAdd = false;
  
            CashVoucher cvv = clist.GetCVIDByNameAndTelNum2(cashVoucher.ReceiverName, cashVoucher.ReceiverTelNo, 2);
            if (cvv.DateTimeIssued.Year == DateTime.Now.Year)
            {
                TempData["readyIssued"] = "You have already issue a cash voucher for this report, you cannot issue again!";
                allowAdd = false;
            }
            else
            {
                allowAdd = true;
            }
            if (allowAdd == true)
            {//$"Follow up with sender for delivery failure  completed by StationMgrSG on {DateTime.Now}"
                DeliveryFailure dfff = dflist.GetOne(par.ParcelID);
                dfff.StationMgrID = cashVoucher.StaffID;
                dfff.FollowUpAction = $"Follow up with sender for delivery failure completed by {stID} on {DateTime.Now}";
                dflist.Update(dfff);
                cashVoucher.CashVoucherID = clist.Add(cashVoucher);
                dhlist.Add(new DeliveryHistory 
                { 
                    ParcelID = dfff.ParcelID,
                    Description = dfff.FollowUpAction
                });

                TempData["Issued"] = "You have yet to issue a cash voucher, you are allow to issue a cash voucher!";
            }
            return View(cashVoucher);

        }

        // POST: CashVoucherController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IssueCompensationVoucher(CashVoucher cashVoucher)
        {
            int yearcheck = DateTime.Now.Year;
            List<CashVoucher> nclist = clist.GetAllCashVoucher();
            
            DeliveryFailure d = null;
            //----------------
            try
            {
                d = JsonConvert.DeserializeObject<DeliveryFailure>((string)TempData["Delivery"]);
            }
            catch
            {
                return View();
            }
           
            return View();
        }
        public ActionResult CheckCashVoucher()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            Member m = mlist.GetMemberfromLoginID((string)HttpContext.Session.GetString("UserID"));
            if (m is null)
            {
                ViewData["Error"] = "Member is null";
                return View();
            }
            List<CashVoucher> cvlist = clist.GetCashVoucherByMemberNotCollect(m);
            
            return View(cvlist);
        }
    }
}
