using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Hotel.Controllers
{
    public class BookingController : Controller
    {
        SqlConnection conn = null;
        string connectionString = "Server=PC-BIT-1444; Database=Hotel; Integrated Security=SSPI";
        // GET: Booking
        public ActionResult Index()
        {
            var accid = TempData["UserID"];
            TempData.Keep("UserID");
            var role = TempData["Role"];
            TempData.Keep("Role");
            if (accid != null && (role.ToString()=="Receptionist" || role.ToString() == "Manager"))
            {
                List<Booking> bookings = new List<Booking>();
                using (conn = new SqlConnection(connectionString))
                {
                    SqlDataReader rd = null;
                    try
                    {
                        conn.Open();
                        string cmdString = "Select b.*,c.*,r.*,t.type,t.price,t.occupants from BookingDetail b, Customer c, Room r, RoomType t where b.CustomerID=c.CustomerID and b.RoomID=r.RoomID and r.RoomTypeID=t.RoomTypeID";
                        SqlCommand cmd = new SqlCommand(cmdString, conn);
                        rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            Booking booking = new Booking
                            {
                                BookingID = Convert.ToInt32(rd["BookingDetailID"].ToString()),
                                BookingReference = rd["bookingReference"].ToString(),
                                BookingMethod = rd["bookingMethod"].ToString(),
                                Date = Convert.ToDateTime(rd["date"].ToString()),
                                Deposit = Convert.ToDecimal(rd["deposit"].ToString()),
                                Checkin = Convert.ToDateTime(rd["checkin"].ToString()),
                                Checkout = Convert.ToDateTime(rd["checkout"].ToString()),
                                CustomerID = Convert.ToInt32(rd["CustomerID"].ToString()),
                                FirstName = rd["firstName"].ToString(),
                                LastName = rd["lastName"].ToString(),
                                Identity = rd["personIdentity"].ToString(),
                                RoomID = Convert.ToInt32(rd["RoomID"].ToString()),
                                RoomNumber = rd["roomNumber"].ToString(),
                                RoomType = rd["type"].ToString(),
                                Price = Convert.ToDecimal(rd["price"].ToString())
                            };
                            bookings.Add(booking);
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Error = e.Message;
                    }
                    rd.Close();
                    conn.Close();
                }

                return View(bookings);
            }
            else
            {
                string url = "http://" + Request.Url.Authority + "/Account/Login";
                return Redirect(url);
            }
        }

        //
        // POST: Booking/OnlineBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnlineBooking(int roomTypeID, string firstName, string lastName, string identity, int rooms, DateTime checkin, DateTime checkout, string cardNumber, string nameHolder, string month, string year, string code)
        {
            string url = "http://" + Request.Url.Authority + "/Home/Index/#booking";
            if (ModelState.IsValid)
            {
                DateTime today = DateTime.Today;
                Random rand = new Random();
                string reference = firstName[0] + lastName + "-" + today.Day + today.Month + today.Year + "-" + rand.Next(1000, 10000);
                decimal total = 0;
                bool isBooked = false;
                using (conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string cmdString="";
                        SqlCommand cmd = null;
                        SqlDataReader rd = null;
                        for (int i=0; i < rooms; i++)
                        {
                            // Retrieve room id
                            int roomId = 0;
                            int status = 0;
                            cmdString = "Select * from Room where RoomTypeID=" + roomTypeID + " and (status=1 or status=2)";
                            cmd = new SqlCommand(cmdString, conn);
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                rd.Read();
                                roomId = Int32.Parse(rd["RoomID"].ToString());
                                status = Int32.Parse(rd["status"].ToString());
                                rd.Close();
                                //Update Room status
                                cmdString = "Update Room ";
                                if (status == 1) cmdString += "Set status=3";
                                else if (status == 2) cmdString += "Set status=4";
                                cmdString += "Where RoomID=" + roomId;
                                cmd = new SqlCommand(cmdString, conn);
                                cmd.ExecuteNonQuery();
                            }
                            rd.Close();

                            if (roomId > 0)
                            {
                                
                                cmdString = "Select * from Customer where personIdentity='" + identity+"'";
                                cmd = new SqlCommand(cmdString, conn);
                                rd = cmd.ExecuteReader();
                                if (!rd.HasRows)
                                {
                                    rd.Close();
                                    //Insert into customer
                                    cmdString = "Insert into Customer(firstName,lastName,personIdentity)";
                                    cmdString += "Values(@firstName,@lastName,@personIdentity)";
                                    cmd = new SqlCommand(cmdString, conn);
                                    cmd.Parameters.Add(new SqlParameter("@firstName", firstName));
                                    cmd.Parameters.Add(new SqlParameter("@lastName", lastName));
                                    cmd.Parameters.Add(new SqlParameter("@personIdentity", identity));
                                    cmd.ExecuteNonQuery();
                                }
                                else rd.Close();
                                
                                // Retrieve new customer id
                                cmdString = "Select * from Customer Where personIdentity='" + identity + "'";
                                cmd = new SqlCommand(cmdString, conn);
                                rd = cmd.ExecuteReader();
                                int cusId = 0;
                                if (rd.HasRows)
                                {
                                    rd.Read();
                                    cusId = Int32.Parse(rd["CustomerID"].ToString());
                                }
                                rd.Close();

                                if (cusId > 0)
                                {
                                    cmdString = "Select price from RoomType where RoomTypeID="+roomTypeID;
                                    cmd = new SqlCommand(cmdString, conn);
                                    rd = cmd.ExecuteReader();
                                    decimal price = 0;
                                    if (rd.Read()) price = Convert.ToDecimal(rd[0].ToString());
                                    total += price;
                                    decimal deposit = price/100*10;
                                    rd.Close();
                                    // Insert into Booking Detail
                                    cmdString = "Insert into BookingDetail(bookingReference,bookingMethod,date,deposit,checkin,checkout,CustomerID,RoomID)";
                                    cmdString += "Values(@bookingReference,@bookingMethod,@date,@deposit,@checkin,@checkout,@CustomerID,@RoomID)";
                                    cmd = new SqlCommand(cmdString, conn);
                                    cmd.Parameters.Add(new SqlParameter("@bookingReference", reference));
                                    cmd.Parameters.Add(new SqlParameter("@bookingMethod", "Online"));
                                    cmd.Parameters.Add(new SqlParameter("@date", today));
                                    cmd.Parameters.Add(new SqlParameter("@deposit", deposit));
                                    cmd.Parameters.Add(new SqlParameter("@checkin", checkin));
                                    cmd.Parameters.Add(new SqlParameter("@checkout", checkout));
                                    cmd.Parameters.Add(new SqlParameter("@CustomerID", cusId));
                                    cmd.Parameters.Add(new SqlParameter("@RoomID", roomId));
                                    cmd.ExecuteNonQuery();
                                }

                                // Retrieve new booking id
                                int bookId = 0;
                                string todayFormat = today.ToShortDateString();
                                todayFormat = Regex.Split(todayFormat, "/")[2] + "-" + Regex.Split(todayFormat, "/")[1] + "-" + Regex.Split(todayFormat, "/")[0];
                                cmdString = "Select MAX(BookingDetailID) From BookingDetail ";
                                cmdString += "Where CustomerID=" + cusId + " and date=cast('" + todayFormat + "' as date)";
                                cmd = new SqlCommand(cmdString, conn);
                                rd = cmd.ExecuteReader();
                                if (rd.Read())
                                    bookId = (Int32.Parse(rd[0].ToString()));

                                rd.Close();

                                if (bookId > 0)
                                {
                                    int cardid = 0;
                                    cmdString = "Select * from CardDetail where cardNumber='"+cardNumber+"'";
                                    cmd = new SqlCommand(cmdString, conn);
                                    rd = cmd.ExecuteReader();
                                    if (!rd.HasRows)
                                    {
                                        rd.Close();
                                        //Insert card detail
                                        cmdString = "Insert Into CardDetail(cardType,cardNumber,nameHolder) ";
                                        cmdString += "Values(@cardType,@cardNumber,@nameHolder)";
                                        cmd = new SqlCommand(cmdString, conn);
                                        cmd.Parameters.Add(new SqlParameter("@cardType", "Credit"));
                                        cmd.Parameters.Add(new SqlParameter("@cardNumber", cardNumber));
                                        cmd.Parameters.Add(new SqlParameter("@nameHolder", nameHolder));
                                        cmd.ExecuteNonQuery();
                                        // Retrieve new cardid
                                        cmdString = "Select * from CardDetail where cardNumber='" + cardNumber + "'";
                                        cmd = new SqlCommand(cmdString, conn);
                                        rd = cmd.ExecuteReader();
                                        while (rd.Read()) cardid = Convert.ToInt32(rd["CardDetailID"].ToString());
                                        rd.Close();
                                        //Update booking
                                        cmdString = "Update BookingDetail Set CardDetailID=" + cardid + " Where BookingDetailID=" + bookId;
                                        cmd = new SqlCommand(cmdString, conn);
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        while(rd.Read()) cardid = Convert.ToInt32(rd["CardDetailID"].ToString());
                                        rd.Close();
                                        cmdString = "Update BookingDetail Set CardDetailID=" + cardid + " Where BookingDetailID=" + bookId;
                                        cmd = new SqlCommand(cmdString, conn);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                TempData["Confirm"] = "Your booking is successful!";
                                isBooked = true;
                            }
                            else
                            {
                                TempData["Confirm"] = "No vacancy room!!!";
                            }
                        }
                        if (isBooked)
                        {
                            cmdString = "Insert Into Invoice(date,total,BookingReference) ";
                            cmdString += "Values(@date,@total,@BookingReference)";
                            cmd = new SqlCommand(cmdString, conn);
                            cmd.Parameters.Add(new SqlParameter("@date", today));
                            cmd.Parameters.Add(new SqlParameter("@total", total));
                            cmd.Parameters.Add(new SqlParameter("@BookingReference", reference));
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    catch(Exception e)
                    {
                        conn.Close();
                        TempData["Confirm"] = " Data Error!!! "+e.Message;
                    }
                }
            }
            else
            {
                TempData["Confirm"] = "Your booking is unsuccessful! Please try again";
            }
            
            return Redirect(url);
        }


    }
    
}