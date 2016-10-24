using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: Booking/OnlineBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnlineBooking(string firstName, string lastName, string identity, DateTime checkin, DateTime checkout, string cardType, string cardNumber, string nameHolder)
        {
            if (ModelState.IsValid)
            {
                string fname = firstName;
                string lname = lastName;
                string id = identity;
                string method = "Online"; 
                DateTime chkin = checkin;
                DateTime chkout = checkout;
                string type = cardType;
                string number = cardNumber;
                string holder = nameHolder;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}