using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;
using System.Composition;

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
        // GET: CashVoucherController1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FailurereportList()
        {
            List<DeliveryFailure> dlist = dflist.GetAllFailureReport();
            return View(dlist);
        }

        public ActionResult CashvoucherList()
        {
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
            /*
            foreach (CashVoucher cashvoucher in cv)
            {
                CashVoucher gcashvoucher = null;
                if (cashvoucher.ReceiverName == rname && cashvoucher.ReceiverTelNo == tnum)
                {
                    TempData["NOResult"] = "";
                    ViewData["showcv"] = true;
                    gcashvoucher =cashvoucher;
                    if (cashvoucher.Status == "0")
                    {
                        gcashvoucher.Status = "Pending Collection";
                    }
                    if (cashvoucher.Status == "1")
                    {
                        gcashvoucher.Status = "Collected";
                    }
                    voucherlist.Add(gcashvoucher);
                }
                else
                { TempData["NOResult"] = "NO Result Found"; }
            }*/

            return View(voucherlist);
        }

        public ActionResult IssueCashVoucherList()
        {
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
            /*
            foreach (CashVoucher c in cc)
            {
                if (c.CashVoucherID == idd)
                {
                    return View(c);
                }
            }*/
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashVoucherUpdate(CashVoucher c)
        {
            c.Status = "1";
            clist.Update(c);
            TempData["SuccessIssued"] = "You have successfully collect your cash voucher";
            //return RedirectToAction("CashvoucherList") ;
            return View();
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
            /*foreach (Member m in mblist)
            {
                if (m.MemberID == id)
                {
                    mm = m; break;
                }
            }*/
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
                /*TempData["cMonth"] = $"Current Month: {thismonth}";
                TempData["mBirthMonth"] = $"Member Birth Day: {birthMonth}";*/
                TempData["IssueAmount"] = "Issue Amount: $10";
            }
            else
            {
                ViewData["CanIssue"] = false;
                ViewData["CannotIssue"] = true;
                /*TempData["cMonth"] = $"Current Month: {thismonth}";
                TempData["mBirthMonth"] = $"Member Birth Day: {birthMonth}";*/
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
            bool allowAdd =false;
            /*foreach (CashVoucher c in nclist)
            {
                if (m.Name == c.ReceiverName && m.TelNo == c.ReceiverTelNo && c.IssuingCode == "1" && c.DateTimeIssued.Year == yearcheck)
                {
                    TempData["readyIssued"] = "Issuse Cash Voucher Failed! You have already Issue cash voucher this year!";
                    allowAdd = false;
                    break;

                }
                else
                {
                    allowAdd = true;
                }
            }
            if (allowAdd == true)
            {
                cashVoucher.CashVoucherID = clist.Add(cashVoucher);
                TempData["Issued"] = "You have successfully issue cash voucher";
            }*/
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
            CashVoucher cashVoucher = new CashVoucher();
            //----------------
            List<DeliveryHistory>dhhlist = dhlist.GetAllHistory();
            List<DeliveryFailure> dlist = dflist.GetAllFailureReport();
            DeliveryFailure df = null;
            foreach (DeliveryFailure dff in dlist)
            {
                if (dff.ReportID == id)
                {
                    df = dff; break;
                }
            }
            //df = dflist.GetOne(id);
            TempData["Delivery"] = JsonConvert.SerializeObject(df);
            //-----------------*******************----------------------------------
            int yearcheck = DateTime.Now.Year;
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
            Parcel par = null;
            List<Parcel> palist = plist.GetAllParcel();
            /*foreach ( Parcel p in palist)
            {
                if (p.ParcelID == d.ParcelID)
                {
                    par = p;
                    cashVoucher.ReceiverName = p.ReceiverName;
                    cashVoucher.ReceiverTelNo = p.ReceiverTelNo;
                    break;
                }
            }*/
            int pd = d.ParcelID;
            par = plist.GetPIDByPID(pd);
            cashVoucher.ReceiverName = par.ReceiverName;
            cashVoucher.ReceiverTelNo = par.ReceiverTelNo;
            cashVoucher.Amount = 20;
            cashVoucher.Currency = "SGD";
            cashVoucher.IssuingCode = "2";

            cashVoucher.DateTimeIssued = DateTime.Now;
            cashVoucher.Status = "0";
            bool allowAdd = false;
            /* foreach (CashVoucher c in nclist)
             {
                 if (cashVoucher.ReceiverName == c.ReceiverName && cashVoucher.ReceiverTelNo == c.ReceiverTelNo && c.IssuingCode == "2" && c.DateTimeIssued.Year == yearcheck)
                 {
                     TempData["readyIssued"] = "You have already issue a cash voucher for this report, you cannot issue again!";
                     allowAdd = false;
                     break;
                 }
                 else
                 {
                     allowAdd = true;
                 }
             }*/
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
            //dfsfhjbdefsivesgudeiogeshboiuejbioeusbhnseighealighiregerogeroriu
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
            
            /*
            string stID = (string)HttpContext.Session.GetString("UserID");

            List<Staff> ls = sdal.GetAllStaff();
            foreach (Staff st in ls)
            {
                if (st.LoginID == stID)
                {
                    cashVoucher.StaffID = st.StaffID;
                    break;
                }
            }
            List<Parcel> palist = plist.GetAllParcel();
            foreach ( Parcel p in palist)
            {
                if (p.ParcelID == d.ParcelID)
                {
                    cashVoucher.ReceiverName = p.ReceiverName;
                    cashVoucher.ReceiverTelNo = p.ReceiverTelNo;
                    break;
                }
            }

            cashVoucher.Amount = 20;
            cashVoucher.Currency = "SGD";
            cashVoucher.IssuingCode = "2";

            cashVoucher.DateTimeIssued = DateTime.Now;
            cashVoucher.Status = "0";
            bool allowAdd = false;
            foreach (CashVoucher c in nclist)
            {
                if (cashVoucher.ReceiverName == c.ReceiverName && cashVoucher.ReceiverTelNo == c.ReceiverTelNo && c.IssuingCode == "2" && c.DateTimeIssued.Year == yearcheck)
                {
                    TempData["readyIssued"] = "Issuse Cash Voucher Failed! You have already Issue cash voucher this year!";
                    allowAdd = false;
                    break;
                }
                else
                {
                    allowAdd = true;
                }
            }
            if (allowAdd == true)
            {
                cashVoucher.CashVoucherID = clist.Add(cashVoucher);
                TempData["Issued"] = "You have successfully issue cash voucher";
            }*/
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
