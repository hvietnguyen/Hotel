using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace Hotel.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection conn = null;
        string connectionString = "Server=PC-BIT-1444; Database=Hotel; Integrated Security=SSPI";
        public ActionResult Index()
        {
            string status = "";
            ViewBag.Confirm = TempData["Confirm"] as string;
            TempData["Confirm"] = "";
            using (conn = new SqlConnection(connectionString))
            {
                try
                {
                    status += GetRoomStatus(1)+".";
                    status += GetRoomStatus(2) + ".";
                    status += GetRoomStatus(3) + ".";
                }
                catch(Exception e)
                {
                    status = e.Message.ToString();
                }
                conn.Close();
                ViewBag.Status = status;
            }
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
        string GetRoomStatus(int roomTypeID)
        {
            string status = "0";
            using (conn = new SqlConnection(connectionString)){
                conn.Open();
                string cmdString = "Select COUNT(*) from Room where status in (1,2) and RoomTypeID=" + roomTypeID;
                SqlCommand cmd = new SqlCommand(cmdString, conn);
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                    status = rd[0].ToString();

                rd.Close();
            }
            conn.Close();
            return status;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}