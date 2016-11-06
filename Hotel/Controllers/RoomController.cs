using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using Hotel.Models;

namespace Hotel.Controllers
{
    public class RoomController : Controller
    {
        SqlConnection conn = null;
        string connectionString = "Server=PC-BIT-1444; Database=Hotel; Integrated Security=SSPI";
        // GET: Room
        public ActionResult Index()
        {
            var accid = TempData["UserID"];
            TempData.Keep("UserID");
            if (accid != null)
            {
                List<Room> rooms = new List<Room>();
                using (conn = new SqlConnection(connectionString))
                {
                    SqlDataReader rd = null;
                    try
                    {
                        conn.Open();
                        string cmdString = "Select r.RoomID,r.roomNumber,r.status,t.type from Room r, RoomType t where r.RoomTypeID=t.RoomTypeID";
                        SqlCommand cmd = new SqlCommand(cmdString, conn);
                        rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            int value = Convert.ToInt32(rd[2].ToString());
                            string status = "";
                            if (value == 1) status = "Vacant Clean";
                            else if (value == 2) status = "Vacant Dirty";
                            else if (value == 3) status = "Occupied Clean";
                            else if (value == 4) status = "Occupied Dirty";
                            else status = "Maintenance";
                            Room room = new Room
                            {
                                RoomID = Convert.ToInt32(rd[0].ToString()),
                                RoomNumber = rd[1].ToString(),
                                Status = status,
                                RoomType = rd[3].ToString()
                            };
                            rooms.Add(room);
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Error = e.Message;
                    }
                    rd.Close();
                    conn.Close();
                }

                return View(rooms);
            }
            else
            {
                string url = "http://" + Request.Url.Authority + "/Account/Login";
                return Redirect(url);
            }
            
        }
    }
}