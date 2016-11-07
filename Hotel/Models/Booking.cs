using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Hotel.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public string BookingReference { get; set; }
        public string BookingMethod { get; set; }
        public DateTime Date { get; set; }
        public decimal Deposit { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Identity { get; set; }
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
    }
}