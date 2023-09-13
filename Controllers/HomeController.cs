using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(int? Page)
        {
            PostController controller = new PostController();
            List<PostModel> posts = new List<PostModel>();

            Page = Page == null ? 1 : Page;

            if (Session["User"] != null)
            {
                posts = controller.GetPosts((Users)Session["User"]);
                ViewBag.Posts = posts.Skip(((int)Page - 1) * 5).Take(5).ToList();
                ViewBag.TotalPages = (int)Math.Ceiling((double)posts.Count() / 5);
            }
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }
    }
}