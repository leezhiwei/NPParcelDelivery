using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class CashVoucherController : Controller
    {
        // GET: CashVoucherController1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CashVoucherstatus()
        {
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
