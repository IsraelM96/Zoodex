using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class AccessController : BaseController
    {
        // Views
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Recover()
        {
            return View();
        }
        public ActionResult ChangePassword(int User, string Token)
        {
            return View();
        }

        //Methods
        public string Encrypt(string password)
        {
            byte[] data = Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);

            return Encoding.ASCII.GetString(data);
        }

        public string GenerateToken()
        {
            string allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string resultToken = new string(
               Enumerable.Repeat(allChar, 16)
               .Select(token => token[random.Next(token.Length)]).ToArray());

            return resultToken.ToString();
        }

        [HttpGet]
        public ActionResult Verify(int User, string Token)
        {
            Users user = new Users();

            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    user = Context.Users.Find(User);
                    if (user.Token == Token)
                    {
                        user.Token = null;
                        user.Verified = true;
                        Context.SaveChanges();
                        WriteLog(User, 1, "User Verify");
                        Session["User"] = user;
                        WriteLog(user.PKUserID, 1, "Log In");
                        TempData["Success"] = "User verified!";
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Errors"] = e.Message;
                WriteLog(User, 0, "In Verify: " + e.Message);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult NewUser()
        {
            Users user = new Users();
            string name = Request.Form["name"];
            string lastName = Request.Form["lastName"];
            string email = Request.Form["email"];
            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    user = Context.Users
                        .Where(a => a.Email == email)
                        .FirstOrDefault();
                    if (user != null)
                    {
                        TempData["Errors"] = "Email already registered";
                        return RedirectToAction("Register");
                    }
                    else
                    {
                        user = Context.Users.Add(new Users
                        {
                            Name = Request.Form["name"],
                            LastName = Request.Form["lastName"],
                            Email = Request.Form["email"],
                            Password = Encrypt(Request.Form["password"]),
                            FKProfilePicture = null,
                            Verified = false,
                            Deleted = false,
                            Token = GenerateToken(),
                            LastUpdated = DateTime.Now
                        });
                        Context.SaveChanges();
                        Context.UsersRoles.Add(new UsersRoles
                        {
                            FKUserID = user.PKUserID,
                            FKRoleID = 1,
                            FKUpdatedBy = 0,
                            Deleted = false,
                            LastUpdated = DateTime.Now
                        });
                        Context.SaveChanges();
                        WriteLog(user.PKUserID, 1, "New User Added");
                        ViewBag.Url = HttpContext.Request.Url.Host + "/" + Url.Action("Verify", "Access") + "?User=" + user.PKUserID + "&Token=" + user.Token;
                        SendMail(email, "Account Verify", RenderPartialToString(ControllerContext, "Mail", null));
                        TempData["Success"] = "Registered User!";
                    }
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In NewUser: " + e.Message);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult Accessed()
        {
            Users user = new Users();
            string email = Request.Form["email"];
            string password = Encrypt(Request.Form["password"]);
            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    user = Context.Users
                        .Where(a => a.Email == email && a.Password == password)
                        .FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Verified)
                        {
                            Session["User"] = user;
                            WriteLog(user.PKUserID, 1, "Log In");
                        }
                        else
                        {
                            TempData["Errors"] = "Please verify you account";
                            return Redirect(Request.UrlReferrer.ToString());
                        }
                    }
                    else
                    {
                        TempData["Errors"] = "Wrong email or password";
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In Accessed: " + e.Message);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Exit()
        {
            Users user = new Users();
            user = (Users)Session["User"];
            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    WriteLog(user.PKUserID, 1, "Log Out");
                    Session.RemoveAll();
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In Exit: " + e.Message);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ResetPassword()
        {
            Users user = new Users();
            string email = Request.Form["email"];
            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    user = Context.Users.Where(x => x.Email == email).FirstOrDefault();
                    user.Token = GenerateToken();
                    Context.SaveChanges();
                    ViewBag.Url = HttpContext.Request.Url.Host + "/" + Url.Action("ChangePassword", "Access") + "?User=" + user.PKUserID + "&Token=" + user.Token;
                    SendMail(email, "Recover Password", RenderPartialToString(ControllerContext, "RecoverPassword", null));
                    TempData["Success"] = "Email sent!";
                }
            }
            catch (Exception e)
            {
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In ResetPassword: " + e.Message);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        public ActionResult UpdatePassword(int User, string Token)
        {
            Users user = new Users();
            string NewPassword = Request.Form["password"];
            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    user = Context.Users.Find(User);
                    if (user.Token == Token)
                    {
                        user.Token = null;
                        user.Password = Encrypt(NewPassword);
                        Context.SaveChanges();
                        WriteLog(User, 1, "Password Reset");
                        TempData["Success"] = "The password has been reset!";
                    }
                    else
                    {
                        TempData["Errors"] = "Token expired!";
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Errors"] = e.Message;
                WriteLog(User, 0, "In UpdatePassword: " + e.Message);
            }

            return RedirectToAction("Index", "Access");
        }
    }
}
