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
            return View();
        }

        //
        // POST: Booking/OnlineBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnlineBooking(int roomTypeID, string firstName, string lastName, string identity, DateTime checkin, DateTime checkout, string cardType, string cardNumber, string nameHolder)
        {
            string url = "http://" + Request.Url.Authority + "/Home/Index/#booking";
            if (ModelState.IsValid)
            {
                using (conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Retrieve room id
                        int roomId=0;
                        int status=0;
                        string cmdString = "Select * from Room where RoomTypeID="+roomTypeID+" and (status=1 or status=2)";
                        SqlCommand cmd = new SqlCommand(cmdString, conn);
                        SqlDataReader rd = cmd.ExecuteReader();
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
                            //Insert into customer
                            cmdString = "Insert into Customer(firstName,lastName,personIdentity)";
                            cmdString += "Values(@firstName,@lastName,@personIdentity)";
                            cmd = new SqlCommand(cmdString, conn);
                            cmd.Parameters.Add(new SqlParameter("@firstName", firstName));
                            cmd.Parameters.Add(new SqlParameter("@lastName", lastName));
                            cmd.Parameters.Add(new SqlParameter("@personIdentity", identity));
                            cmd.ExecuteNonQuery();

                            // Retrieve new customer id
                            cmdString = "Select * from Customer Where personIdentity='" + identity+"'";
                            cmd = new SqlCommand(cmdString, conn);
                            rd = cmd.ExecuteReader();
                            int cusId = 0;
                            if (rd.HasRows)
                            {
                                rd.Read();
                                cusId = Int32.Parse(rd["CustomerID"].ToString());
                            }
                            rd.Close();

                            DateTime today = DateTime.Today;
                            if (cusId > 0)
                            {
                                // Insert into Booking Detail
                                cmdString = "Insert into BookingDetail(bookingMethod,date,checkin,checkout,CustomerID,RoomID)";
                                cmdString += "Values(@bookingMethod,@date,@checkin,@checkout,@CustomerID,@RoomID)";
                                cmd = new SqlCommand(cmdString, conn);
                                cmd.Parameters.Add(new SqlParameter("@bookingMethod", "Online"));
                                cmd.Parameters.Add(new SqlParameter("@date", today));
                                cmd.Parameters.Add(new SqlParameter("@checkin", checkin));
                                cmd.Parameters.Add(new SqlParameter("@checkout", checkout));
                                cmd.Parameters.Add(new SqlParameter("@CustomerID", cusId));
                                cmd.Parameters.Add(new SqlParameter("@RoomID", roomId));
                                cmd.ExecuteNonQuery();
                            }

                            // Retrieve new booking id
                            int bookId = 0;
                            string todayFormat = today.ToShortDateString();
                            todayFormat = Regex.Split(todayFormat, "/")[2]+"-"+ Regex.Split(todayFormat, "/")[1] +"-"+ Regex.Split(todayFormat, "/")[0];
                            cmdString = "Select BookingDetailID From BookingDetail ";
                            cmdString += "Where CustomerID=" + cusId + " and date=cast('" + todayFormat+"' as date)";
                            cmd = new SqlCommand(cmdString, conn);
                            rd = cmd.ExecuteReader();
                            if (rd.Read())
                                bookId = (Int32.Parse(rd[0].ToString()));

                            rd.Close();

                            if(bookId > 0)
                            {
                                //Insert card detail
                                cmdString = "Insert Into CardDetail(cardType,cardNumber,nameHolder,BookingDetailID) ";
                                cmdString += "Values(@cardType,@cardNumber,@nameHolder,@BookingDetailID)";
                                cmd = new SqlCommand(cmdString, conn);
                                cmd.Parameters.Add(new SqlParameter("@cardType", cardType));
                                cmd.Parameters.Add(new SqlParameter("@cardNumber", cardNumber));
                                cmd.Parameters.Add(new SqlParameter("@nameHolder", nameHolder));
                                cmd.Parameters.Add(new SqlParameter("@BookingDetailID", bookId));
                                cmd.ExecuteNonQuery();
                                TempData["Confirm"] = "Your booking is successful!";
                            }
                        }
                        else
                        {
                            TempData["Confirm"] = "No vacancy room!!!";
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