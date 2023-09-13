using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class BaseController : Controller
    {
        public Zoodex_Entities Context = new Zoodex_Entities();
        public string GetHostName()
        {
            return Dns.GetHostEntry(HttpContext.Request.UserHostAddress).HostName.Split('.')[0].ToUpper();
        }
        public string GetIP()
        {
            return HttpContext.Request.UserHostAddress;
        }
        public static string RenderPartialToString(ControllerContext controller, string viewName, object model)
        {
            controller.Controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller, viewName);
                ViewContext viewContext = new ViewContext(controller, viewResult.View, controller.Controller.ViewData, controller.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        public static void SendMail(string to, string subject, string body)
        {
            var Message = new MailMessage();
            Message.IsBodyHtml = true;
            Message.To.Add(new MailAddress(to));
            Message.From = new MailAddress(WebConfigurationManager.AppSettings["EmailAccount"]);
            Message.Subject = subject;
            Message.Body = body;
            Message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credencial = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["EmailAccount"],
                    Password = WebConfigurationManager.AppSettings["EmailPassword"],
                };

                smtp.Credentials = credencial;
                smtp.Host = WebConfigurationManager.AppSettings["EmailHost"];
                smtp.EnableSsl = true;
                smtp.Send(Message);
            }
        }
        public static void SendMail(List<string> to, List<string> cc, string subject, string body)
        {
            var Message = new MailMessage();
            Message.IsBodyHtml = true;
            foreach (string mail in to)
            {
                Message.To.Add(new MailAddress(mail));
            }
            foreach (string mail in cc)
            {
                Message.CC.Add(new MailAddress(mail));
            }
            Message.From = new MailAddress(WebConfigurationManager.AppSettings["EmailAccount"]);
            Message.Subject = subject;
            Message.Body = body;
            Message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credencial = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["EmailAccount"],
                    Password = WebConfigurationManager.AppSettings["EmailPassword"],
                };

                smtp.Credentials = credencial;
                smtp.Host = WebConfigurationManager.AppSettings["EmailHost"];
                smtp.EnableSsl = true;
                smtp.Send(Message);
            }
        }
        public void WriteLog(int UserID, int Status, string Result)
        {
            try
            {
                using (Context)
                {
                    Context.Logs.Add(new Logs
                    {
                        FKUser = UserID,
                        Status = Status,
                        Result = Result,
                        HostName = GetHostName(),
                        IP = GetIP(),
                        Deleted = false,
                        LastUpdated = DateTime.Now
                    });
                    Context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
        }
    }
}
