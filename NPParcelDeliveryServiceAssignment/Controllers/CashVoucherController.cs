using Microsoft.AspNetCore.Http;
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
        // GET: CashVoucherController1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CashVoucherstatus()
        {
            ViewData["showcv"] = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashVoucherstatus(IFormCollection form)
        {
            ViewData["showcv"] = false;
            List<CashVoucher> cv = clist.GetAllCashVoucher();
            string rname = (form["nameBox"]);
            string tnum = (form["telBox"]);
            CashVoucher gcashvoucher = null;
            foreach (CashVoucher cashvoucher in cv)
            {
                if (cashvoucher.ReceiverName == rname && cashvoucher.ReceiverTelNo == tnum)
                {
                    TempData["NOResult"] = "";
                    ViewData["showcv"] = true;
                    gcashvoucher = cashvoucher;
                    if (cashvoucher.Status == "0")
                    {
                        gcashvoucher.Status = "Pending Collection";
                    }
                    if (cashvoucher.Status == "1")
                    {
                        gcashvoucher.Status = "Collected";
                    }
                }
                else
                { TempData["NOResult"] = "NO Result Found"; }
            }

            return View(gcashvoucher);
        }

        public ActionResult IssueCashVoucherList()
        {
			List<Member> mblist = mlist.GetAllMember();
			return View(mblist);
        }

        public ActionResult CashVoucherUpdate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashVoucherUpdate(CashVoucher cashVoucher)
        {
            cashVoucher.Status = "1";
            clist.Update(cashVoucher);
            TempData["collectcv"] = "You have successfully collect your cash voucher";
            return RedirectToAction("CashVoucherstatus") ;
        }
            // GET: CashVoucherController1/Details/5
            public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CashVoucherController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CashVoucherController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
		public ActionResult IssueSpecialVoucher(int id)
		{
            ViewData["CanIssue"] = false;
            ViewData["CannotIssue"] = false;
			//----------------
			List<Member>mblist = mlist.GetAllMember();
            Member mm = null;
            foreach (Member m in mblist)
            {
                if (m.MemberID == id)
                {
                    mm = m; break;
                }
            }
            if (mm is null)
            {
                ViewData["error"] = $"Error";
                return View();
            }

            TempData["Member"] = JsonConvert.SerializeObject(mm);

            DateTime now = DateTime.Now;
            int thismonth = Convert.ToInt32(now.Month.ToString());
            string mbirthDay = mm.BirthDate.ToString();
            string[] birthMonthh = mbirthDay.Split("/");
            int birthMonth = Convert.ToInt32(birthMonthh[0]);
            if (thismonth == birthMonth)
            {
                ViewData["CanIssue"] = true;
                ViewData["CannotIssue"] = false;
                TempData["cMonth"] = $"Current Month: {thismonth}";
                TempData["mBirthMonth"] = $"Member Birth Day: {birthMonth}";
                TempData["IssueAmount"] = "Issue Amount: $10";
            }
            else
            {
                ViewData["CanIssue"] = false;
                ViewData["CannotIssue"] = true;
                TempData["cMonth"] = $"Current Month: {thismonth}";
                TempData["mBirthMonth"] = $"Member Birth Day: {birthMonth}";
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
            int moncheck = DateTime.Now.Month;
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
            foreach (Staff st in ls)
            {
                if (st.LoginID == stID)
                {
                    cashVoucher.StaffID = st.StaffID;
                    break;
                }
            }

            cashVoucher.Amount = 10;
            cashVoucher.Currency = "SGD";
            cashVoucher.IssuingCode = "1";
            cashVoucher.ReceiverName = m.Name;
            cashVoucher.ReceiverTelNo = m.TelNo;
            cashVoucher.DateTimeIssued = DateTime.Now;
            cashVoucher.Status = "0";
            foreach (CashVoucher c in nclist)
            {
                if (m.Name == c.ReceiverName && m.TelNo == c.ReceiverTelNo && c.IssuingCode == "1" && c.DateTimeIssued.Month == moncheck)
                {
                    TempData["readyIssued"] = "Issuse Cash Voucher Failed! You have already Issue cash voucher this year!";

                }
                else
                {
                    cashVoucher.CashVoucherID = clist.Add(cashVoucher);
                    TempData["SuccessIssued"] = "Successfully Issued cash voucher!"; break;
                }
            }
			return View();
		}

		// GET: CashVoucherController1/Edit/5
		public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CashVoucherController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CashVoucherController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CashVoucherController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
