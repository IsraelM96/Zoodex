using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zoodex.Functions;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class AdminController : BaseController
    {
        public ActionResult Logs()
        {
            Users user = (Users)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (RoleHandler.GetRole(user.PKUserID) != "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Logs = GetLogs();
            return View();
        }

        public ActionResult Database()
        {
            Users user = (Users)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (RoleHandler.GetRole(user.PKUserID) != "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public List<LogModel> GetLogs()
        {
            Users user = (Users)Session["User"];
            List<LogModel> logs = new List<LogModel>();
            try
            {
                using (Context)
                {
                    logs = Context.Logs.Where(a => a.Deleted == false)
                        .Select(a => new LogModel
                        {
                            Id = a.PKLogID,
                            User = Context.Users
                                    .Where(b => b.PKUserID == a.FKUser && b.Deleted == false)
                                    .Select(b => b.Name + " " + b.LastName)
                                    .FirstOrDefault(),
                            Status = a.Status == 1 ? "Success" : "Error",
                            Result = a.Result,
                            Hostname = a.HostName,
                            IP = a.IP,
                            Date = a.LastUpdated
                        }).OrderByDescending(a => a.Date).ToList();
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In GetLogs: " + e.Message);
            }

            return logs;
        }
    }
}