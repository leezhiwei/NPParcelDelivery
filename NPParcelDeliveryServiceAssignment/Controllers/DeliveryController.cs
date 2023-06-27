using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPParcelDeliveryServiceAssignment.Models;
using NPParcelDeliveryServiceAssignment.DALs;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class DeliveryController : Controller
    {
        private ParcelDAL pdal = new ParcelDAL();
        private ShippingRateDAL srd = new ShippingRateDAL();
        // GET: DeliveryController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DeliveryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }   

        // GET: DeliveryHistory/Insert
        public ActionResult Insert()
        {
            return View();
        }

        // POST: DeliveryHistory/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(IFormCollection collection)
        {
            return RedirectToAction("Insert");
        }

        // GET: DeliveryController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DeliveryController/Edit/5
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

        // GET: DeliveryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DeliveryController/Delete/5
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
        public ActionResult ShowShippingRateInfo()
        {
            List<ShippingRate> ShippingRatelist = srd.GetAllShippingRate();
            return View(ShippingRatelist);
        }
        public ActionResult AssignParcels()
        {
            ViewData["Error"] = false;
            ViewData["ShowParcel"] = false;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignParcels(IFormCollection form)
        {
            List<Parcel> lp = pdal.GetAllParcel();
            Parcel p = null;
            int pid = 0;
            try
            {
                pid = Convert.ToInt32(form["idbox"]);
            }
            catch
            {
                ViewData["ShowParcel"] = false;
                ViewData["Error"] = true;
                return View();
            }
            foreach (Parcel parcel in lp)
            {
                if (parcel.ParcelID == pid)
                {
                    p = parcel;
                }
            }
            if (p is not null)
            {
                ViewData["ShowParcel"] = true;
                ViewData["Error"] = false;
                return View(p);
            }
            ViewData["ShowParcel"] = false;
            ViewData["Error"] = true;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateParcel(IFormCollection form)
        {
            TempData["MessageSuccess"] = "You have successfully updated the database";
            return RedirectToAction("AssignParcels");
        }
        public ActionResult CreateShippingRate()
        {
            return View();
        }
    }
}
