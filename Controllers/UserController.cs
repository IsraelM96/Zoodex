using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Profile()
        {
            if(Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateProfilePicture()
        {
            Users user = (Users)Session["User"];
            Files file = new Files();

            try
            {
                HttpPostedFileBase currentFile = Request.Files[0];
                string fileName = "User_" + user.PKUserID + "_Profile_Picture.png";
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Storage/Profile Pictures"), fileName);
                currentFile.SaveAs(filePath);

                using (Context)
                {
                    if (user.FKProfilePicture == null)
                    {
                        Context.Files.Add(new Files
                        {
                            FileName = fileName,
                            Path = "https://zoodex.azurewebsites.net/Profile" + " Pictures/" + fileName,
                            FKUpdatedBy = user.PKUserID,
                            LastUpdated = DateTime.Now,
                            Deleted = false
                        });

                        Context.SaveChanges();

                        file = Context.Files.Where(a => a.FileName == fileName && a.Deleted == false).FirstOrDefault();
                        user = Context.Users.Where(a => a.PKUserID == user.PKUserID && a.Deleted == false).FirstOrDefault();

                        user.FKProfilePicture = file.PKFileID;
                        user.LastUpdated = DateTime.Now;
                        Context.SaveChanges();
                    }
                    else
                    {
                        file = Context.Files.Where(a => a.FileName == fileName && a.Deleted == false).FirstOrDefault();
                        user = Context.Users.Where(a => a.PKUserID == user.PKUserID && a.Deleted == false).FirstOrDefault();

                        file.LastUpdated = DateTime.Now;
                        user.FKProfilePicture = file.PKFileID;
                        user.LastUpdated = DateTime.Now;
                        Context.SaveChanges();
                    }

                    Session["User"] = user;

                    WriteLog(user.PKUserID, 1, "Profile Picture Updated!");
                    TempData["Success"] = "Profile Picture Updated!";
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In UpdateProfilePicture: " + e.Message);
            }

            return RedirectToAction("Profile", "User");
        }

        [HttpPost]
        public ActionResult UpdateInformation()
        {
            Users user = (Users)Session["User"];

            try
            {
                string name = Request.Form["name"];
                string lastName = Request.Form["lastName"];
                string password = Encrypt(Request.Form["password"]);
                string email = Request.Form["email"];
                int rol = Convert.ToInt32(Request.Form["rol"]);

                using (Context)
                {
                    if (user.Password == password)
                    {
                        user = Context.Users.Where(a => a.PKUserID == user.PKUserID && a.Deleted == false).FirstOrDefault();

                        user.Name = name; 
                        user.LastName = lastName;
                        user.Email = email;
                        user.LastUpdated = DateTime.Now;

                        Context.UsersRoles.Where(a => a.FKUserID == user.PKUserID && a.Deleted == false).FirstOrDefault().FKRoleID = rol;

                        Context.SaveChanges();
                        WriteLog(user.PKUserID, 1, "User Information Updated!");
                        TempData["Success"] = "User Information Updated!";

                        Session["User"] = user;
                    }
                    else
                    {
                        WriteLog(user.PKUserID, 0, "In UpdateInformation: invalid password");
                        TempData["Errors"] = "Invalid Password!";
                    }
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In UpdateInformation: " + e.Message);
            }

            return RedirectToAction("Profile", "User");
        }

        public string Encrypt(string password)
        {
            byte[] data = Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);

            return Encoding.ASCII.GetString(data);
        }
    }
}
