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
            var role = TempData["Role"];
            TempData.Keep("Role");
            if (accid != null && (role.ToString() == "Receptionist" || role.ToString() == "Manager" || role.ToString() == "RoomKeeper"))
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

        public ActionResult Details(int id)
        {
            Room room = null;
            using (conn = new SqlConnection(connectionString))
            {
                SqlDataReader rd = null;
                try
                {
                    conn.Open();
                    string cmdString = "Select * from Room r, RoomType t where r.RoomTypeID=t.RoomTypeID and RoomID="+id;
                    SqlCommand cmd = new SqlCommand(cmdString, conn);
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        int value = Convert.ToInt32(rd["status"].ToString());
                        string status = "";
                        if (value == 1) status = "Vacant Clean";
                        else if (value == 2) status = "Vacant Dirty";
                        else if (value == 3) status = "Occupied Clean";
                        else if (value == 4) status = "Occupied Dirty";
                        else status = "Maintenance";
                        room = new Room
                        {
                            RoomID = Convert.ToInt32(rd[0].ToString()),
                            RoomNumber = rd["roomNumber"].ToString(),
                            Status = status,
                            RoomType = rd["type"].ToString()
                        };
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
                rd.Close();
                conn.Close();
            }

            return View(room);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int roomID, int status)
        {
            using (conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string cmdString = "Update Room Set status="+status+" Where RoomID="+roomID;
                    SqlCommand cmd = new SqlCommand(cmdString, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
                conn.Close();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRoom(string roomNumber, int type)
        {
            using (conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string cmdString = "Insert into Room(roomNumber, status, RoomTypeID) ";
                    cmdString += "Values(@roomNumber,@status,@RoomTypeID)";
                    SqlCommand cmd = new SqlCommand(cmdString, conn);
                    cmd.Parameters.Add(new SqlParameter("@roomNumber", roomNumber));
                    cmd.Parameters.Add(new SqlParameter("@status", 5));
                    cmd.Parameters.Add(new SqlParameter("@RoomTypeID", type));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
                conn.Close();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            using (conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string cmdString = "Delete From Room Where RoomID="+id;
                    SqlCommand cmd = new SqlCommand(cmdString, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
                conn.Close();
            }
            return RedirectToAction("Index");
        }
    }
}