using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPParcelDeliveryServiceAssignment.Models;
using NPParcelDeliveryServiceAssignment.DALs;
using Newtonsoft.Json;
using DeepEqual.Syntax;
using Microsoft.Extensions.Logging.Abstractions;
using NuGet.Packaging.Signing;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class DeliveryController : Controller
    {
        private CashVoucherDAL cvdal = new CashVoucherDAL();
        private PaymentDAL paydal = new PaymentDAL();
        private ParcelDAL pdal = new ParcelDAL();
        private DeliveryHistoryDAL dhdal = new DeliveryHistoryDAL();
        private ShippingRateDAL srd = new ShippingRateDAL();
        private StaffDAL sdal = new StaffDAL();
        private DeliveryFailureDAL dfdal = new DeliveryFailureDAL();
        private List<string> ft = new List<string> { "Receiver not found", "Wrong delivery addresss", "Parcel damaged", "Other" };
        private List<SelectListItem> list = new List<SelectListItem>();
        private MemberDAL mdal = new MemberDAL();
        private List<string> country = new List<string> { "Singapore", "Malaysia", "Indonesia", "China", "USA", "Japan", "France", "UK", "Australia" };
        private List<SelectListItem> PopulateCVlist()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "Cash",
                Value = "CASH"
            });
            list.Add(new SelectListItem
            {
                Text = "Voucher",
                Value = "VOUC"
            });
            return list;
        }
        private void PopulateList()
        {
            foreach (string types in ft)
            {
                list.Add(new SelectListItem { Text = types, Value = (ft.IndexOf(types) + 1).ToString() });
            }
        }
        // GET: DeliveryController/ParcelHistory
        public ActionResult DeliveryHistory()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<DeliveryHistory> dhList = dhdal.GetAllHistory();
            return View(dhList);
        }


        // POST: Parcel Delivery History
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeliveryHistory(IFormCollection form)
        {
            List<DeliveryHistory> dh = dhdal.GetAllHistory();
            List<DeliveryHistory> dhTemp = new List<DeliveryHistory>();
            int rId = 0;
            try
            {
                rId = Convert.ToInt32(form["rIdSearch"]);
            }
            catch (Exception ex) //Catch if the textbox is empty and display error msg
            {
                TempData["rError"] = $"The search textbox is empty, contains letters or special characters, unable to search for any delivery history!  Please enter numeric values.";
                return View(dh);
            }
            foreach (DeliveryHistory dhh in dh)
            {
                if (dhh.RecordID == rId)
                {
                    dhTemp.Add(dhh); //If recordID matches the record in the list, add record to deliveryhistoryTemp for viewing
                }
            }

            if (dhTemp.Count > 0) //If deliveryhistoryTemp is NOT empty and contains information
            {
                TempData["rFound"] = $"Delivery History with the record ID: {rId}, found.";
                return View(dhTemp);
            }
            else //prints the error msg that record is not found, since the deliveryhistoryTemp is empty
            {
                TempData["rError"] = $"Delivery History with the ID: {rId}, does not exist in the delivery history records.";
                return View(dh);
            }
        }
        private List<SelectListItem> GetCountries()
        {
            List<SelectListItem> countries = new List<SelectListItem>();
            countries.Add(new SelectListItem
            {
                Value = null,
                Text = "--- Select Country ---"
            });
            countries.Add(new SelectListItem
            {
                Value = "Australia",
                Text = "Australia"
            });
            countries.Add(new SelectListItem
            {
                Value = "China",
                Text = "China"
            });
            countries.Add(new SelectListItem
            {
                Value = "France",
                Text = "France"
            });
            countries.Add(new SelectListItem
            {
                Value = "Indonesia",
                Text = "Indonesia"
            });
            countries.Add(new SelectListItem
            {
                Value = "Japan",
                Text = "Japan"
            });
            countries.Add(new SelectListItem
            {
                Value = "Malaysia",
                Text = "Malaysia"
            });
            countries.Add(new SelectListItem
            {
                Value = "Singapore",
                Text = "Singapore"
            });
            countries.Add(new SelectListItem
            {
                Value = "UK",
                Text = "UK"
            });
            countries.Add(new SelectListItem
            {
                Value = "USA",
                Text = "USA"
            });
            return countries;
        }

        public ActionResult Insert()
        {
            ViewData["Countries"] = GetCountries();
            Parcel p = new Parcel //Setting default values
            {
                Currency = "SGD",
                ParcelWeight = 0.0,
                DeliveryStatus = "0"
            };
            return View(p);
        }

        // POST: Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(Parcel p)
        {
            ShippingRate ccObject = srd.GetSRbyCC(p.ToCity, p.ToCountry); //Creating a shipping rate object to inherit a shipping rate object that has the same tocity & tocountry

            decimal deliveryCharge = 0;
            decimal rdelCharge = 0;
            decimal shipRate = 0;
            int transitTime = 0;

            if (ccObject.IsDeepEqual(new ShippingRate())) // Checks if the ccObject equals to a new shippingrate that has empty values
            {
                TempData["ErrorMessage"] = $"Parcel creation failed. <br><br>------------------------------------------------------------ <br><br> Invalid ToCity & ToCountry, please try again with the correct city & country names. <br><br> ------------------------------------------------------------";
                return RedirectToAction("Insert");
            }
            else if ((p.ToCity.ToLower() == ccObject.ToCity.ToLower()) && (p.ToCountry.ToLower() == ccObject.ToCountry.ToLower())) //Checks if the city & country matches the records in shipping rate 
            {
                //Advanced Feature 3 - Parcel Receiving, compute delivery charge
                shipRate = ccObject.ShipRate; //Store shiprate into shipRate, to be printed out later as tempData
                deliveryCharge = Convert.ToDecimal(p.ParcelWeight) * ccObject.ShipRate; //Delivery Charge = parcel weight * ship rate
                //Basic Feature 2 - Parcel Receiving, calculating target delivery date
                p.ToCity = ccObject.ToCity; //Added to replace value entered by staff. E.g. if staff enter pAriS, it will be replaced to Paris from shipping rate.
                p.ToCountry = ccObject.ToCountry; //Added to replace value entered by staff. E.g. if staff enter frAnCe, it will be replaced to France from shipping rate.
                transitTime = ccObject.TransitTime;
            }

            //Advanced Feature 3 - Parcel Receiving, compute delivery charge
            rdelCharge = Math.Round(deliveryCharge, MidpointRounding.AwayFromZero); //Rounding the delivery charge to the nearest dollar
            if (rdelCharge >= 5) //Checks if delivery charge is more than 5
            {
                p.DeliveryCharge = rdelCharge;
            }
            else //If delivery charge is less than 5, the minimum delivery charge is 5 dollars  
            {
                p.DeliveryCharge = 5;
            }

            //Basic Feature 2 - Parcel Receiving, calculating target delivery date
            DateTime receiveParcel = DateTime.Now;
            DateTime tdd = receiveParcel.AddDays(transitTime); //Target delivery date = receive parcel datetime + transit datetime
            p.TargetDeliveryDate = tdd;


            //Basic Feature 1 - Parcel Receiving, adding parcel delivery record
            string desc = $"Recieved parcel by {HttpContext.Session.GetString("UserID")} on {DateTime.Now.ToString("dd MMM yyyy hh:mm tt")}.";

            DeliveryHistory dh = new DeliveryHistory
            {
                ParcelID = pdal.Add(p), //Obtaining parcel ID by adding details to parcel
                Description = desc,
            };
            dhdal.Add(dh); //Adding parcel ID & description into delivery history

            TempData["InsertMessage"] = $"Parcel Added to Database! <br><br> ------------------ Parcel Delivery Order ------------------ <br><br> Parcel ID:  {p.ParcelID} <br> Parcel Weight:  {p.ParcelWeight} kg <br> From City and Country:  {p.FromCity}, {p.FromCountry} <br> To City and Country:  {p.ToCity}, {p.ToCountry} <br> Shipping Rate:  {String.Format("{0:0.##}", shipRate)}/kg <br> Delivery Charge (Raw):  ({String.Format("{0:0.##}", shipRate)} x {p.ParcelWeight}) = ${String.Format("{0:0.##}", deliveryCharge)} <br> Delivery Charge (Rounded):  ${String.Format("{0:0.##}", rdelCharge)} <br> Delivery Charge (Final):  ${String.Format("{0:0.##}", p.DeliveryCharge)} (Note: Minimum delivery charge is S$5.00) <br><br> ------------------------------------------------------------";
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
            /*foreach (ShippingRate s in sr)
            {
                if (s.ShippingRateID == idd)
                {
                    TempData["PrevObj"] = JsonConvert.SerializeObject(s);
                    return View(s);
                }
            }*/
            if (srd.GetSRIDByID(id).ShippingRateID == idd)
            {
                TempData["PrevObj"] = JsonConvert.SerializeObject(srd.GetSRIDByID(id));
                return View(srd.GetSRIDByID(id));
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
            /*foreach (Staff st in ls)
            {
                if (st.LoginID == loginID)
                {
                    s.LastUpdatedBy = st.StaffID;
                    break;
                }
            }*/
            int staffIDCheck = sdal.ReturnStaffID(loginID);
            if (staffIDCheck <= -1)
            {
                return View();
            }
            else
            { s.LastUpdatedBy = sdal.ReturnStaffID(loginID); }
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
            /*foreach (ShippingRate s in sr)
            {
                if (s.ShippingRateID == id)
                {
                    TempData["shiprateobj"] = JsonConvert.SerializeObject(s);
                    return View(s);
                }
            }*/
            if (srd.GetSRIDByID(Convert.ToString(id)).ShippingRateID == id)
            {
                TempData["shiprateobj"] = JsonConvert.SerializeObject(srd.GetSRIDByID(Convert.ToString(id)));
                return View(srd.GetSRIDByID(Convert.ToString(id)));
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
            ViewData["SelectList"] = pdal.GetDManCount();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignParcels(IFormCollection form)
        {
            ViewData["SelectList"] = pdal.GetDManCount();
            Parcel p = new Parcel();
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
            p = pdal.GetPIDByPID(pid);
            if (p.IsDeepEqual(new Parcel()))
            {
                ViewData["ShowParcel"] = false;
                ViewData["Error"] = true;
                return View();
            }
            ViewData["ShowParcel"] = true;
            ViewData["Error"] = false;
            return View(p);
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
                TempData["Error2"] = "Invalid input.";
                return RedirectToAction("AssignParcels");
            }
            int count = pdal.GetCountFromStaffID(staffid);
            if (count >= 5)
            {
                TempData["Error2"] = "More than 5 parcel set. Please fufil more deliveries.";
                return RedirectToAction("AssignParcels");
            }

            Staff s = sdal.GetOneStaff(staffid);
            if (s is null)
            {
                TempData["Error2"] = "Invalid StaffID!";
                return RedirectToAction("AssignParcels");
            }
            if (s.Appointment != "Delivery Man")
            {
                TempData["Error2"] = "The selected Staff is not a delivery man";
                return RedirectToAction("AssignParcels");
            }
            void Merge(Parcel existingobject, Parcel somevalues)
            {
                // From stackoverflow, https://stackoverflow.com/questions/8702603/merging-two-objects-in-c-sharp, Reflection method
                Type t = typeof(Parcel);
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
            int? rcount = 0;
            Parcel pa = pdal.GetPIDByPID(p.ParcelID);
            Merge(pa, p);
            rcount = pdal.Update(p);

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
            List<SelectListItem> ge = GetCountries();
            ViewData["country"]=ge;
            return View();
        }

        // POST: shipping rate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateShippingRate(ShippingRate shippingRate)
        {
            List<SelectListItem> ge = GetCountries();
            ViewData["country"] = ge;
            if (ModelState.IsValid)
            {
                List<ShippingRate> shippingRateList = srd.GetAllShippingRate();
                if (srd.IsInfoExist(shippingRate))
                {
                    TempData["Fail"] = "Error Such Ship Rate Info is already Exisit!";
                    return View();
                }
                string loginID = HttpContext.Session.GetString("UserID");
                List<Staff> staffli = sdal.GetAllStaff();
                /*foreach (Staff s in staffli)
                {
                    if (s.LoginID == loginID)
                    {
                        shippingRate.LastUpdatedBy = Convert.ToInt32(s.StaffID);
                    }
                }*/
                int staffIDCheck = sdal.ReturnStaffID(loginID);
                if (staffIDCheck <= -1)
                {
                    return View();
                }
                else
                { shippingRate.LastUpdatedBy = sdal.ReturnStaffID(loginID); }
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
                return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
            }
            Parcel pobj = pdal.GetPIDByPID(pid);
            if (pobj == new Parcel())
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParcelDeliveryOrder(IFormCollection form)
        {
            List<Parcel> pl = pdal.GetAllParcel();
            List<Parcel> ppTemp = new List<Parcel>();
            int pId = 0;
            try
            {
                pId = Convert.ToInt32(form["pIdSearch"]);
            }
            catch (Exception ex) //Catch if the textbox is empty and display error msg
            {
                TempData["ParcelError"] = $"The search textbox is empty, contains letters or special characters, unable to search for any existing parcels!  Please enter numeric values.";
                return View(pl);
            }
            foreach (Parcel parcel in pl)
            {
                if (parcel.ParcelID == pId)
                {
                    ppTemp.Add(parcel); //If parcel matches the record in the list, add parcel to tempparcel for viewing
                }
            }

            if (ppTemp.Count > 0) //If tempparcel is NOT empty and contains information
            {
                TempData["ParcelFound"] = $"Parcel with the ID: {pId}, found.";
                return View(ppTemp);
            }
            else //prints the error msg that parcel is not found, since the tempparcel is empty
            {
                TempData["ParcelError"] = $"Parcel with the ID: {pId}, does not exist in the delivery orders.";
                return View(pl);
            }
        }
        public ActionResult Report()
        {
            PopulateList();
            ViewData["SListI"] = list;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(DeliveryFailure df)
        {
            PopulateList();
            ViewData["SListI"] = list;
            df.DateCreated = DateTime.Now;
            string loginid = HttpContext.Session.GetString("UserID");
            int staffid = sdal.ReturnStaffID(loginid);
            df.DeliveryManID = staffid;
            if (dfdal.CheckIfSimilar(df))
            {
                ViewData["Error"] = "Cannot insert a similar deliveryfailure report.";
                return View();
            }
            Parcel parcel = pdal.GetPIDByPID(df.ParcelID);
            if (parcel == new Parcel())
            {
                ViewData["Error"] = "Unable to get Parcel Object";
                return View();
            }
            int dmanid = 0;
            try
            {
                dmanid = (int)parcel.DeliveryManID;
            }
            catch
            {
                ViewData["Error"] = "Unable to convert.";
                return View();
            }
            if (dmanid != df.DeliveryManID)
            {
                ViewData["Error"] = "This parcel is not assigned to you.";
                return View();
            }
            if (parcel.DeliveryStatus != "4")
            {
                ViewData["Error"] = "This parcel has not failed delivery, please check again.";
                return View();
            }
            dfdal.Add(df);
            ViewData["Success"] = "You have successfully updated the database.";
            return View();
        }
        public ActionResult FailedDel(string id)
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
                return RedirectToAction("Index", "Home");
            }
            Parcel pobj = pdal.GetPIDByPID(pid);
            if (pobj == new Parcel())
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["Parcel"] = JsonConvert.SerializeObject(pobj);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FailedDel()
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
            p.DeliveryStatus = "4";
            pdal.Update(p);
            dhdal.Add(new DeliveryHistory
            {
                ParcelID = p.ParcelID,
                Description = $"Parcel has been failed to be delivered by {HttpContext.Session.GetString("UserID")} on {DateTime.Now.ToString("dd MMM yyyy hh:mm tt")}."
            });
            TempData["Success"] = "You have successfully updated the database.";
            return RedirectToAction("List");
        }

        public ActionResult DeliverySearch()
        {
            ViewData["ShowDetail"] = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeliverySearch(IFormCollection form)
        {
            List<Parcel> pl = pdal.GetAllParcel();
            List<Parcel> parcelTemp = new List<Parcel>();
            Parcel pTemp = null;
            string ParcelId;
            string rname;
            string sname;
            int pid = 0;


            ParcelId = form["ParcelID"];
            rname = form["rnameSearch"];
            sname = form["snameSearch"];
            try
            {
                pid = Convert.ToInt32(ParcelId);
            }
            catch
            {
                pid = 0;
            }
            foreach (Parcel parcel in pl)
            {
                if (parcel.ParcelID == pid || parcel.ReceiverName == rname || parcel.SenderName == sname)
                {
                    parcelTemp.Add(parcel); //If parcel matches the record in the list, add parcel to tempparcel for viewing
                    pTemp = parcel;
                    break;
                }
            }

            if (parcelTemp.Count > 0) //If tempparcel is NOT empty and contains information
            {
                TempData["ParcelFound"] = $"Parcel with the ID: {ParcelId}, found.";
                ViewData["ShowDetail"] = true;
                if (pTemp.DeliveryStatus == "0")
                {
                    TempData["ParcelStatus"] = "Pending Delivery";
                }
                else if (pTemp.DeliveryStatus == "1")
                {
                    TempData["ParcelStatus"] = "Delivery to Destination in Progress";
                }
                else if (pTemp.DeliveryStatus == "2")
                {
                    TempData["ParcelStatus"] = "Delivery to Airport in Progress";
                }
                else if (pTemp.DeliveryStatus == "3")
                {
                    TempData["ParcelStatus"] = "Delivery Completed";
                }
                else if (pTemp.DeliveryStatus == "4")
                {
                    TempData["ParcelStatus"] = "Delivery Failed";
                }
                else
                {
                    TempData["ParcelStatus"] = " ";
                }
                return View(pTemp);
            }
            else //prints the error msg that parcel is not found, since the tempparcel is empty
            {
                TempData["ParcelError"] = $"Parcel with the ID: {ParcelId}, does not exist in the delivery orders.";
                ViewData["ShowDetail"] = false;
                return View(pTemp);
            }
        }



        public ActionResult PaymentTransaction()
        {
            ViewData["TranType"] = PopulateCVlist();
            /*
            List<SelectListItem> templist = PopulateCVlist();
            PaymentTransaction pt = new PaymentTransaction //Setting default values
            {
                Currency = "SGD",
                TranType = Convert.ToString(templist[0])
            };
            return View(pt);
            */
            return View();
        }



        // POST: Payment Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentTransaction(PaymentTransaction pt)
        {
            List<SelectListItem> templist = PopulateCVlist();
            List<Parcel> pcls = pdal.GetAllParcel();
            List<CashVoucher> cv = cvdal.GetAllCashVoucher();
            List<PaymentTransaction> ptsn = paydal.GetAllPayment();
            //Advanced Feature 4 - Parcel Receiving, Create Payment Transaction

            string sName = "";
            decimal? tAmt;

            Parcel tempParcel = pdal.GetPIDByPID(pt.ParcelID); //Creating a temporary parcel to hold information from parcel id
            

            if(tempParcel is not null) //Checks if tempParcel is not null and contains information
            {
                sName = tempParcel.SenderName;
                tAmt = paydal.GetSumAmtByID(pt.ParcelID); //Total Amount is the sum of all previous payments made by the person either cash or voucher

                if (tAmt is null) //If total amount is null, that means the person has not made any previous payment with cash or voucher
                {
                    tAmt = 0; //Total Amount value will be 0
                }

                if (pt.TranType == "VOUC") // Transaction type is voucher
                {
                    CashVoucher tempVoucher = cvdal.GetCVIDByName(sName); //Creates a temporary voucher to hold information from name
                    if (tempVoucher is null) //Checks if temporary voucher is null. If so, the user currently do not own any vouchers
                    {
                        TempData["ErrorMsg"] = "You do not own any vouchers at the moment. Please choose a different transaction type.";
                        return RedirectToAction("PaymentTransaction");
                    }
                    else // If temporary voucher contains information. If so, the current user has vouchers to be used
                    {
                        if (pt.AmtTran > tempVoucher.Amount) //Checks if amount exceeds available voucher
                        {
                            TempData["ErrorMsg"] = "Transaction amount exceeded available cash voucher! Please try again.";
                            return RedirectToAction("PaymentTransaction");
                        }
                        else if ((pt.AmtTran + Convert.ToDecimal(tAmt)) > tempParcel.DeliveryCharge) //Checks if amount + total amount made by previous payment exceeds delivery charge
                        {
                            TempData["ErrorMsg"] = "Transaction amount exceeded delivery charge. You are not ALLOWED to pay extra! Please try again.";
                            return RedirectToAction("PaymentTransaction");
                        }
                        // Update voucher details, if user used $5 voucher out of $10. Update remaining voucher details
                        decimal originalVamt = cvdal.GetVByName(sName);
                        decimal tempUVamt = tempVoucher.Amount - pt.AmtTran; //Temporary updated voucher amount = total voucher amount - voucher transaction amount used
                        decimal updatedVamt = cvdal.UpdateCVbyNameAmount( tempUVamt,sName);
                        TempData["Details"] = $"---------------------------------- <br><br> Voucher amount used: ${String.Format("{0:0}", pt.AmtTran)} <br> Original Voucher amount: ${String.Format("{0:0}", originalVamt)} <br> Updated Voucher amount: ${String.Format("{0:0}", updatedVamt)} <br><br> ----------------------------------"; 
                    }
                }
                else // Transaction type is cash
                {
                    if ((pt.AmtTran + Convert.ToDecimal(tAmt)) > tempParcel.DeliveryCharge)
                    {
                        TempData["ErrorMsg"] = "Transaction Amount exceeded delivery charge. You are not ALLOWED to pay extra! Please try again.";
                        return RedirectToAction("PaymentTransaction");
                    }
                }
            }
            else
            {
                TempData["ErrorMsg"] = $"Parcel ID: {pt.ParcelID} does not EXIST.";
                return RedirectToAction("PaymentTransaction");
            }

            PaymentTransaction pts = new PaymentTransaction
            {
                ParcelID = pt.ParcelID,
                AmtTran = pt.AmtTran,
                Currency = pt.Currency,
                TranType = pt.TranType,
                TranDate = DateTime.Now, //Sets the transaction date to current time
            };
            paydal.Add(pts); //Adding payment details into payment transaction       

            TempData["SuccessMsg"] = $"Payment Transaction with Parcel ID: {pt.ParcelID} is successfull!";
            return RedirectToAction("PaymentTransaction");


        }

        public ActionResult DeliveryHist(int? id)
        {
            int tempid = 0;
            try
            {
                tempid = Convert.ToInt32(id);
            }
            catch
            {
                return View(new List<DeliveryHistory>());
            }
            List<DeliveryHistory> vList = dhdal.GetParcelHistory(tempid);
            return View(vList);
        }

        public IActionResult CustomerTracking()
        {
            Member m = mdal.GetMemberfromLoginID(HttpContext.Session.GetString("UserID"));
            List<Parcel> plist = pdal.GetParcelFromMember(m);
            return View(plist);
        }


        // GET: Payment Transaction
        public ActionResult PaymentTransactionHistory()
        {
            if (HttpContext.Session.GetString("UserID") is null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PaymentTransaction> pth = paydal.GetAllPayment();
            return View(pth);
        }

        // POST: Payment Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentTransactionHistory(IFormCollection form)
        {
            List<PaymentTransaction> pt = paydal.GetAllPayment();
            List<PaymentTransaction> ptnTemp = new List<PaymentTransaction>();
            int tId = 0;
            try
            {
                tId = Convert.ToInt32(form["tIdSearch"]);
            }
            catch (Exception ex) //Catch if the textbox is empty and display error msg
            {
                TempData["tError"] = $"The search textbox is empty, contains letters or special characters, unable to search for any payment transaction!  Please enter numeric values.";
                return View(pt);
            }
            foreach (PaymentTransaction ptn in pt)
            {
                if (ptn.TransactionID == tId)
                {
                    ptnTemp.Add(ptn); //If transaction matches the record in the list, add transaction to tempTransaction for viewing
                }
            }

            if (ptnTemp.Count > 0) //If tempTransaction is NOT empty and contains information
            {
                TempData["tFound"] = $"Payment Transaction with the ID: {tId}, found.";
                return View(ptnTemp);
            }
            else //prints the error msg that transaction is not found, since the tempparcel is empty
            {
                TempData["tError"] = $"Payment Transaction with the ID: {tId}, does not exist in the transaction history.";
                return View(pt);
            }
        }

    }
}
