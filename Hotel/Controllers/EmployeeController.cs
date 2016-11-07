using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using Hotel.Models;

namespace Hotel.Controllers
{
    public class EmployeeController : Controller
    {
        SqlConnection conn = null;
        string connectionString = "Server=PC-BIT-1444; Database=Hotel; Integrated Security=SSPI";
        // GET: Employee
        public ActionResult Index()
        {
            var accid = TempData["UserID"];
            TempData.Keep("UserID");
            var role = TempData["Role"];
            TempData.Keep("Role");
            if (accid != null && role.ToString()=="Manager")
            {
                List<Employee> employees = new List<Employee>();
                using (conn = new SqlConnection(connectionString))
                {
                    SqlDataReader rd = null;
                    try
                    {
                        conn.Open();
                        string cmdString = "Select e.EmployeeID,e.firstName,e.lastName, e.personIdentity, e.contact,e.address,a.AccountID,a.roleName,a.account From Employee e, Account a where a.AccountID=e.AccountID";
                        SqlCommand cmd = new SqlCommand(cmdString, conn);
                        rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            Employee e = new Employee
                            {
                                EmployeeID = Convert.ToInt32(rd["EmployeeID"].ToString()),
                                FirstName = rd["firstName"].ToString(),
                                LastName = rd["lastName"].ToString(),
                                Identity = rd["personIdentity"].ToString(),
                                Contact = rd["contact"].ToString(),
                                Address = rd["address"].ToString(),
                                AccountID = rd["AccountID"].ToString(),
                                Role = rd["roleName"].ToString(),
                                Account = rd["account"].ToString()
                            };
                            employees.Add(e);
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Error = e.Message;
                    }
                    rd.Close();
                    conn.Close();
                }

                return View(employees);
            }
            else
            {
                string url = "http://" + Request.Url.Authority + "/Account/Login";
                return Redirect(url);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string firstName, string lastName, string identity, string contact, string address, string role)
        {
            using (conn = new SqlConnection(connectionString))
            {
                SqlDataReader rd = null;
                SqlCommand cmd;
                try
                {
                    conn.Open();
                    string cmdString = "Insert Into Account(account,roleName,pass) ";
                    cmdString += "Values(@account,@roleName,@pass)";
                    Random rnd = new Random();
                    string acc = firstName[0] + lastName + rnd.Next(1000, 10000);
                    string pass = lastName[0] + DateTime.Today.Year.ToString();
                    cmd = new SqlCommand(cmdString, conn);
                    cmd.Parameters.Add(new SqlParameter("@account", acc));
                    cmd.Parameters.Add(new SqlParameter("@roleName", role));
                    cmd.Parameters.Add(new SqlParameter("@pass", pass));
                    cmd.ExecuteNonQuery();

                    cmdString="Select * from Account where account='"+acc+"'";
                    cmd = new SqlCommand(cmdString, conn);
                    rd = cmd.ExecuteReader();
                    int accid = 0;
                    if (rd.HasRows)
                    {
                        rd.Read();
                        accid = Convert.ToInt32(rd["AccountID"].ToString());
                    }
                    rd.Close();

                    cmdString = "Insert Into Employee(firstName,lastName,personIdentity,contact,address,AccountID) ";
                    cmdString += "Values(@firstName,@lastName,@personIdentity,@contact,@address,@AccountID)";
                    cmd = new SqlCommand(cmdString, conn);
                    cmd.Parameters.Add(new SqlParameter("@firstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", lastName));
                    cmd.Parameters.Add(new SqlParameter("@personIdentity", identity));
                    cmd.Parameters.Add(new SqlParameter("@contact", contact));
                    cmd.Parameters.Add(new SqlParameter("@address", address));
                    cmd.Parameters.Add(new SqlParameter("@AccountID", accid));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
                
                conn.Close();
            }
            string url = "http://" + Request.Url.Authority + "/Employee/Index";
            return Redirect(url);
        }
        public ActionResult Delete(int id)
        {
            using (conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string cmdString = "Select * from Employee where EmployeeID="+id;
                    SqlCommand cmd = new SqlCommand(cmdString, conn);
                    SqlDataReader rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        int accid = Convert.ToInt32(rd["AccountID"].ToString());
                        rd.Close();
                        cmdString = "Delete From Employee Where EmployeeID=" + id;
                        cmd = new SqlCommand(cmdString, conn);
                        cmd.ExecuteNonQuery();

                        cmdString = "Delete From Account Where AccountID=" + accid;
                        cmd = new SqlCommand(cmdString, conn);
                        cmd.ExecuteNonQuery();
                    }
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