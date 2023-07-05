using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPParcelDeliveryServiceAssignment.Models;
using NPParcelDeliveryServiceAssignment.DALs;
using Newtonsoft.Json;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class DeliveryController : Controller
    {
        private ParcelDAL pdal = new ParcelDAL();
        private DeliveryDAL dhdal = new DeliveryDAL();
        private ShippingRateDAL srd = new ShippingRateDAL();
        private StaffDAL sdal = new StaffDAL();

        // GET: DeliveryController/ParcelHistory
        public ActionResult DeliveryHistory()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<DeliveryHistory> dhList = dhdal.GetAllHistory();
            return View(dhList);
            //return View();
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
        public ActionResult Insert()
        {
            return View();
        }
        // POST: Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(IFormCollection collection)
        {   
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
                DeliveryCharge = 0,
                Currency = collection["Currency"],
                TargetDeliveryDate = null,
                DeliveryStatus = collection["DeliveryStatus"],
                DeliveryManID = Convert.ToInt32(collection["DeliveryManID"]),
            };

            List<ShippingRate> SP = srd.GetAllShippingRate();
            //Advanced Feature 3
            decimal dc = 0;
            foreach (ShippingRate s in SP)
            {
                if ((p.ToCity.ToLower() == s.ToCity.ToLower()) && (p.ToCountry.ToLower() == s.ToCountry.ToLower())) //Checks if the city & country matches the records in shipping rate 
                {
                    dc = Convert.ToDecimal(p.ParcelWeight) * s.ShipRate; //Delivery Charge = parcel weight * ship rate
                    break;
                }
            }
            dc = Math.Round(dc, MidpointRounding.AwayFromZero); //Rounding the delivery charge to the nearest dollar
            if (dc >= 5) //Checks if delivery charge is more than 5
            {
                p.DeliveryCharge = dc;
            }
            else //If delivery charge is less than 5, the minimum delivery charge is 5 dollars  
            {
                p.DeliveryCharge = 5;
            }
            

            //Basic Feature 2, calculating target delivery date
            int tt = 0;
            foreach (ShippingRate s in SP)
            {
                if ((p.ToCity.ToLower() == s.ToCity.ToLower()) && (p.ToCountry.ToLower() == s.ToCountry.ToLower())) //Checks if the city & country matches the records in shipping rate 
                {
                    p.ToCity = s.ToCity; //Added to replace value entered by staff. E.g. if staff enter pAriS, it will be replaced to Paris from shipping rate.
                    p.ToCountry = s.ToCountry; //Added to replace value entered by staff. E.g. if staff enter frAnCe, it will be replaced to France from shipping rate.
                    tt = s.TransitTime;
                    break;
                }
            }
            DateTime receiveParcel = DateTime.Now;
            DateTime tdd = receiveParcel.AddDays(tt); //Target delivery date = receive parcel datetime + transit datetime
            p.TargetDeliveryDate = tdd;


            //Basic Feature 1, adding parcel delivery record
            string desc = $"Recieved parcel by {HttpContext.Session.GetString("UserID")} on {DateTime.Now.ToString("dd MMM yyyy hh:mm tt")}.";

            DeliveryHistory dh = new DeliveryHistory
            {
                ParcelID = pdal.Add(p), 
                Description = desc,
            };
            dhdal.Add(dh); //Adding parcel ID & description into delivery history

            TempData["InsertMessage"] = "Parcel Added to Database";
            return RedirectToAction("Insert");


        }

        // GET: DeliveryController/Edit/5
        public ActionResult ShippingRateEdit(string id)
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            int idd = 0;
            try
            {
                idd = Convert.ToInt32(id);
            }
            catch
            {
                return RedirectToAction("ShowShippingRateInfo");
            }
            List<ShippingRate> sr = srd.GetAllShippingRate();
            foreach (ShippingRate s in sr)
            {
                if (s.ShippingRateID == idd)
                {
                    TempData["PrevObj"] = JsonConvert.SerializeObject(s);
                    return View(s);
                }
            }
            return View();
        }

        // POST: DeliveryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShippingRateEdit(ShippingRate s)
        {
            ShippingRate oldobj = null;
            try
            {
                oldobj = JsonConvert.DeserializeObject<ShippingRate>((string)TempData["PrevObj"]);
            }
            catch
            {
                return View();
            }
            void Merge(ShippingRate existingobject, ShippingRate somevalues)
            {
                // From stackoverflow, https://stackoverflow.com/questions/8702603/merging-two-objects-in-c-sharp, Reflection method
                Type t = typeof(ShippingRate);
                // get type obj of ShippingRate
                var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
                // get property of obj
                foreach (var prop in properties)
                { // foreach property
                    var value = prop.GetValue(somevalues);
                    // get the value from the "blank" from form object, some value null or 0
                    if (value is null)
                    { // if the value is indeed null
                        var valuefromexistingobj = prop.GetValue(existingobject);
                        // get this data from the "existing" object
                        prop.SetValue(somevalues, valuefromexistingobj);
                        // set it into the new object
                        continue; // move on to next property
                    }
                    try // try to convert to int, exception move to catch
                    { // if above never trigger, check for ID 0 or number 0
                        int? numberval = (int)value; // convert to int 
                        if (numberval == 0)
                        { // if the numbervalue is 0, eg id
                            var valuefromexistingobj = prop.GetValue(existingobject);
                            // get this data from "existing" object.
                            prop.SetValue(somevalues, valuefromexistingobj);
                            // set it into new object
                            continue; // move on to next property
                        }
                    }
                    catch // catch the exception
                    {
                        continue; // move to new object
                    }
                }
            }
            Merge(oldobj, s);
            string loginID = (string)HttpContext.Session.GetString("UserID");
            List<Staff> ls = sdal.GetAllStaff();
            foreach (Staff st in ls)
            {
                if (st.LoginID == loginID)
                {
                    s.LastUpdatedBy = st.StaffID;
                    break;
                }
            }
            srd.Update(s);
            TempData["UpdateSuccess"] = "You have successfully update the shipping rate";
            return View();

        }

        // GET: DeliveryController/Delete/5
        public ActionResult DeleteShippingRate(int? id)
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ShippingRate> sr = srd.GetAllShippingRate();
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
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ShippingRate> ShippingRatelist = srd.GetAllShippingRate();
            return View(ShippingRatelist);
        }
        public ActionResult AssignParcels()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
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
            List<Parcel> plist = pdal.GetAllParcel();
            int staffid = 0;
            try
            {
                staffid = (int)p.DeliveryManID;
            }
            catch
            {
                TempData["Error2"] = "No Deliveryman set.";
                return View();
            }
            int count = 0;
            foreach (Parcel pa in plist)
            {
                if (pa.DeliveryManID == staffid)
                {
                    count++;
                }
            }
            if (count == 5)
            {
                TempData["Error2"] = "More than 5 parcel set. Please fufil more deliveries.";
                return View();
            }
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
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
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
                string loginID = HttpContext.Session.GetString("UserID");
                List<Staff>staffli = sdal.GetAllStaff();
                foreach (Staff s in staffli)
                {
                    if (s.LoginID == loginID)
                    {
                        shippingRate.LastUpdatedBy = Convert.ToInt32(s.StaffID);
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
        public ActionResult List()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            string LoginID = HttpContext.Session.GetString("UserID");
            int StaffID = sdal.ReturnStaffID(LoginID);
            if (StaffID == -1)
            {
                return RedirectToAction("Index","Home");
            }
            List<Parcel> AssignedList = pdal.CheckAssigned(StaffID);
            return View(AssignedList);
        }
        public ActionResult CompleteDel(string id)
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            int pid = 0;
            try
            {
                pid = Convert.ToInt32(id);
            }
            catch
            {
                return RedirectToAction("Index","Home");
            }
            Parcel pobj = pdal.ReturnParcel(pid);
            if (pobj is null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["Parcel"] = JsonConvert.SerializeObject(pobj);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteDel()
        {
            Parcel p = null;
            try
            {
                p = JsonConvert.DeserializeObject<Parcel>((string)TempData["Parcel"]);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            if ((p.ToCity == p.FromCity) && (p.ToCountry == p.FromCountry))
            {
                p.DeliveryStatus = "3";
                pdal.Update(p);
                dhdal.Add(new DeliveryHistory
                {
                    ParcelID = p.ParcelID,
                    Description = $"Parcel delivered successfully by {HttpContext.Session.GetString("UserID")} on {DateTime.Now.ToString("dd MMM yyyy hh:mm tt")}."
                });
                TempData["Success"] = "You have successfully delivered the parcel, database recorded.";
                return RedirectToAction("List");
            }
            else
            {
                p.DeliveryStatus = "2";
                pdal.Update(p);
                dhdal.Add(new DeliveryHistory
                {
                    ParcelID = p.ParcelID,
                    Description = $"Parcel delivered to airport by {HttpContext.Session.GetString("UserID")} on {DateTime.Now.ToString("dd MMM yyyy hh:mm tt")}."
                });
                TempData["Success"] = "You have successfully delivered the parcel to the airport, database recorded.";
                return RedirectToAction("List");
            }
        }

        public ActionResult ParcelDeliveryOrder()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            //return View();
            List<Parcel> pList = pdal.GetAllParcel();
            return View(pList);
        }
    }
}
