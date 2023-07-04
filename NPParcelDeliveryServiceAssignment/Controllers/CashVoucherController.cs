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
            }

            return View(gcashvoucher);
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
