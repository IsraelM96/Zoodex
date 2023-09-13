using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class ReptilesController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public JsonResult GetFullReptiles()
        {
            List<Reptiles> ReptilesList = new List<Reptiles>();
            try
            {
                Context.Configuration.LazyLoadingEnabled = false;
                using (Context)
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    serializer.MaxJsonLength = 900000000;
                    
                    ReptilesList = Context.Reptiles
                        .Where(a => a.Deleted == false)
                        .ToList();

                    var json = Json(ReptilesList, JsonRequestBehavior.AllowGet);
                    json.MaxJsonLength = 900000000;

                    return json;
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return null;
        }
    }
}
