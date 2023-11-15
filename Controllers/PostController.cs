using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zoodex.Models;
using static System.Net.WebRequestMethods;

namespace Zoodex.Controllers
{
    public class PostController : BaseController
    {
        public ActionResult Create()
        {
            if (Session["User"] != null)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult View(int Id)
        {
            if (Session["User"] != null)
            {
                ViewBag.Post = GetPost(Id);
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult CreatePost()
        {
            Users user = (Users)Session["User"];
            Posts post = new Posts();
            Files file = new Files();
            string title = Request.Form["title"];
            string content = Request.Form["content"];
           
            try
            {
                
                using(Context)
                {
                    post = Context.Posts.OrderByDescending(a => a.PKPostID).FirstOrDefault();

                    HttpPostedFileBase currentFile = Request.Files[0];
                    string fileName = post == null ? "Post_1_Image." + currentFile.FileName.Split('.')[1] : "Post_" + post.PKPostID + "_Image." + currentFile.FileName.Split('.')[1];
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/Storage/Posts"), fileName);
                    currentFile.SaveAs(filePath);

                    Context.Files.Add(new Files
                    {
                        FileName = fileName,
                        Path = "https://zoodex.azurewebsites.net/Storage/Posts/" + fileName,
                        FKUpdatedBy = user.PKUserID,
                        LastUpdated = DateTime.Now,
                        Deleted = false
                    });

                    Context.SaveChanges();

                    file = Context.Files.Where(a => a.FileName == fileName && a.Deleted == false).FirstOrDefault();

                    Context.Posts.Add(new Posts
                    {
                        Title = title,
                        Content = content,
                        FKPicture = file.PKFileID,
                        FKUpdatedBy = user.PKUserID,
                        LastUpdated = DateTime.Now,
                        Deleted = false
                    });

                    Context.SaveChanges();
                    WriteLog(user.PKUserID, 1, "Post Created!");
                    TempData["Success"] = "Post Created!";
                }
            }
            catch (Exception e)
            {
                user = user != null ? user : new Users();
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In CreatePost: " + e.Message);
            }
            return RedirectToAction("Index", "Home");
        }

        public List<PostModel> GetPosts(Users user)
        {
            List<Posts> posts = new List<Posts>();
            List<PostModel> PostList = new List<PostModel>();
            try
            {
                using (Context)
                {
                    posts = Context.Posts.Where(a => a.Deleted == false).OrderByDescending(a => a.PKPostID).ToList();
                    foreach(Posts post in posts)
                    {
                        Users PostedBy = Context.Users.Where(a => a.PKUserID == post.FKUpdatedBy).FirstOrDefault();
                        Files Image = Context.Files.Where(a => a.PKFileID == post.FKPicture).FirstOrDefault();
                        PostList.Add(new PostModel
                        {
                            Id = post.PKPostID,
                            Title = post.Title,
                            PostedById = PostedBy.PKUserID,
                            ProfileImageId = PostedBy.FKProfilePicture,
                            PostedBy = PostedBy.GetFullName(),
                            Image = Image.Path,
                            Content = post.Content,
                            Created = post.LastUpdated,
                            Deleted = false
                        });
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In GetPosts: " + e.Message);
            }

            return PostList;
        }

        public PostModel GetPost(int Id)
        {
            Users user = (Users)Session["User"];
            Posts post = new Posts();
            PostModel PostModel = new PostModel();
            try
            {
                using (Context)
                {
                    post = Context.Posts.Where(a => a.PKPostID == Id && a.Deleted == false).FirstOrDefault();
                    Users PostedBy = Context.Users.Where(a => a.PKUserID == post.FKUpdatedBy).FirstOrDefault();
                    Files Image = Context.Files.Where(a => a.PKFileID == post.FKPicture).FirstOrDefault();
                    PostModel.Id = post.PKPostID;
                    PostModel.Title = post.Title;
                    PostModel.PostedById = PostedBy.PKUserID;
                    PostModel.PostedBy = PostedBy.GetFullName();
                    PostModel.ProfileImageId = PostedBy.FKProfilePicture;
                    PostModel.Image = Image.Path;
                    PostModel.Content = post.Content;
                    PostModel.Created = post.LastUpdated;
                    PostModel.Deleted = false;
                }
            }
            catch (Exception e)
            {
                TempData["Errors"] = e.Message;
                WriteLog(user.PKUserID, 0, "In GetPost: " + e.Message);
            }

            return PostModel;
        }
    }
}
