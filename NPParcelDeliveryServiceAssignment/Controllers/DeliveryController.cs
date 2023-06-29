using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPParcelDeliveryServiceAssignment.Models;
using NPParcelDeliveryServiceAssignment.DALs;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using NuGet.Protocol.Core.Types;
using Newtonsoft.Json;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class DeliveryController : Controller
    {
        private ParcelDAL pdal = new ParcelDAL();
        private DeliveryDAL dhdal = new DeliveryDAL();
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
            //HttpContext.Session.SetString("ParcelReceiveTime", DateTime.Now.ToString());
            //string desc = $"Recieved parcel by {HttpContext.Session.GetString("LoginID")} on {HttpContext.Session.GetString("ParcelReceiveTime")}.";

            Parcel p = new Parcel 
            {
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

            /*DeliveryHistory dh = new DeliveryHistory
            {
                Description = desc,
            };
            dhdal.Add(dh);*/

            return RedirectToAction("Insert");


        }

        // GET: DeliveryController/Edit/5
        public ActionResult ShippingRateEdit(int? id)
        {
            List<ShippingRate>sr = srd.GetAllShippingRate();
            foreach (ShippingRate s in sr)
            {
                if (s.ShippingRateID == id)
                {
                    TempData["editShipRate"] = JsonConvert.SerializeObject(s);
                    return View(s);
                }
            }
            return View();
        }

        // POST: DeliveryController/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]


        public ActionResult ShippingRateEdit()
        {
               ShippingRate s = JsonConvert.DeserializeObject<ShippingRate>(TempData["editShipRate"].ToString());
               srd.Update(s);
               TempData["UpdateSuccess"] = "You have successfully update the shipping rate";
               return View();

        }

        // GET: DeliveryController/Delete/5
        public ActionResult DeleteShippingRate(int? id)
        {
            List<ShippingRate>sr = srd.GetAllShippingRate();
            foreach (ShippingRate s in sr)
            {
                if (s.ShippingRateID == id)
                {
                    TempData["shiprateobj"] = JsonConvert.SerializeObject(s);
                    return View(s);
                }
            }
            return View();
        }

        // POST: DeliveryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteShippingRate(ShippingRate shippingRate)
        {
            ShippingRate s = JsonConvert.DeserializeObject<ShippingRate>(TempData["shiprateobj"].ToString());
			srd.Delete(s.ShippingRateID);
            return RedirectToAction("ShowShippingRateInfo");
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
        public ActionResult UpdateParcel(Parcel p)
        {
            p.DeliveryStatus = "1"; // set deliverystatus to in progress
            int? rcount = pdal.Update(p);
            if (rcount is null)
            {
                TempData["NotFound"] = $"There is no StaffID matching {p.DeliveryManID}";
                return RedirectToAction("AssignParcels");
            }
            dhdal.Add(new DeliveryHistory
            {
                ParcelID = p.ParcelID,
                Description = $"Received parcel by {HttpContext.Session.GetString("UserID")} on {DateTime.Now.ToString("dd MMM yyyy hh:mm tt")}"
            });
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
                List<ShippingRate> shippingRateList = srd.GetAllShippingRate();
                foreach (ShippingRate s in shippingRateList)
                {
                    if (s.ToCity == shippingRate.ToCity && s.ToCountry == shippingRate.ToCountry
                        && s.FromCity == shippingRate.FromCity && s.FromCountry == shippingRate.FromCountry)
                    {
                        TempData["ErrorMessage"] = "Error Such Ship Rate Info is already Exisit!";
                        return View();
                    }

                }
                //Add staff record to database
                shippingRate.ShippingRateID = srd.Add(shippingRate);//.Add(shippingRate);
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
