using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPParcelDeliveryServiceAssignment.Models;
using NPParcelDeliveryServiceAssignment.DALs;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class DeliveryController : Controller
    {
        private ParcelDAL pdal = new ParcelDAL();
        private ShippingRateDAL srd = new ShippingRateDAL();
        private StaffDAL sdal = new StaffDAL();
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
        // GET: shipping rate/Create
        public ActionResult Create()
        {
            return View();
        }
        private List<SelectListItem> GetCountries()
        {
            List<SelectListItem> countries = new List<SelectListItem>();
            countries.Add(new SelectListItem
            {
                Value = "Singapore",
                Text = "Singapore"
            });
            countries.Add(new SelectListItem
            {
                Value = "Malaysia",
                Text = "Malaysia"
            });
            countries.Add(new SelectListItem
            {
                Value = "Indonesia",
                Text = "Indonesia"
            });
            countries.Add(new SelectListItem
            {
                Value = "China",
                Text = "China"
            });

            return countries;
        }
        // POST: DeliveryHistory/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(IFormCollection collection)
        {
            Parcel p = new Parcel {
                ItemDescription = collection["ItemDescription"],
                SenderName = collection["SenderName"],
                SenderTelNo = collection["SenderTelNo"],
                ReceiverName = collection["ReceiverName"],
                ReceiverTelNo = collection["ReceiverTelNo"],
                DeliveryAddress = collection["DeliveryAddress"],
                FromCity = collection["FromCity"],
                FromCountry = collection["FromCountry"],
                ToCity = collection["ToCity"],
                ToCountry = collection["ToCountry"],
                ParcelWeight = Convert.ToDouble(collection["ParcelWeight"]),
                DeliveryCharge = Convert.ToDecimal(collection["DeliveryCharge"]),
                Currency = collection["Currency"],
                TargetDeliveryDate = Convert.ToDateTime(collection["TargetDeliveryDate"]),
                DeliveryStatus = collection["DeliveryStatus"],
                DeliveryManID = Convert.ToInt32(collection["DeliveryManID"]),
            };

            pdal.Add(p);

            return RedirectToAction("Insert");


        }

        // GET: DeliveryController/Edit/5
        public ActionResult ShippingRateEdit()
        {
            return View();
        }

        // POST: DeliveryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShippingRateEdit(IFormCollection form)
        {
            ShippingRate s = new ShippingRate
            {
                ShippingRateID = Convert.ToInt32(form["ShippingRateID"]),
                FromCity = form["FromCity"],
                FromCountry = form["FromCountry"],
                ToCity = form["ToCity"],
                ToCountry = form["ToCountry"],
                ShipRate = Convert.ToDecimal(form["ShipRate"]),
                Currency = form["Currency"],
                TransitTime = Convert.ToInt32(form["TransitTime"]),
                LastUpdatedBy = Convert.ToInt32(form["LastUpdatedBy"])
		    };
            return RedirectToAction("Delivery", "ShowShippingRateInfo");
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

        // POST: shipping rate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateShippingRate(ShippingRate shippingRate)
        {
            if (ModelState.IsValid)
            {
                //Add staff record to database
                shippingRate.ShippingRateID = srd.Add(shippingRate);//.Add(shippingRate);
                                                                    //Redirect user to Staff/Index view
                TempData["CreateSuccess"] = "You have successfully create a new shipping rate";
				return View();

            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(shippingRate);
            }
        }
    }
}
