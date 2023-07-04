using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPParcelDeliveryServiceAssignment.DALs;
using NPParcelDeliveryServiceAssignment.Models;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class CashVoucherController : Controller
    {
        private CashVoucherDAL clist = new CashVoucherDAL();
        // GET: CashVoucherController1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CashVoucherstatus()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashVoucherstatus(IFormCollection form)
        {
            List<CashVoucher> cv = clist.GetAllCashVoucher();
            string rname = (form["nameBox"]);
            string tnum = (form["telBox"]);
            CashVoucher gcashvoucher = null;
            foreach (CashVoucher cashvoucher in cv)
            {
                if (cashvoucher.ReceiverName == rname && cashvoucher.ReceiverTelNo == tnum)
                {
                    gcashvoucher = cashvoucher;
                    /*gcashvoucher.CashVoucherID = cashvoucher.CashVoucherID;
                    gcashvoucher.DateTimeIssued = cashvoucher.DateTimeIssued;
                    gcashvoucher.Currency = cashvoucher.Currency;
                    gcashvoucher.IssuingCode = cashvoucher.IssuingCode;
                    gcashvoucher.Status = cashvoucher.Status;
                    gcashvoucher.StaffID = cashvoucher.StaffID;
                    gcashvoucher.ReceiverName = cashvoucher.ReceiverName;
                    gcashvoucher.ReceiverTelNo = cashvoucher.ReceiverTelNo;
                    gcashvoucher.Amount = cashvoucher.Amount;*/
                }
            }
            return View(gcashvoucher);
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
