using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zoodex.Models;

namespace Zoodex.Controllers
{
    public class AnalyzeController : BaseController
    {
        // GET: Analyze
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Predict()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpPostedFileBase currentFile = Request.Files[0];
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Storage/Analysis"), currentFile.FileName);
                currentFile.SaveAs(filePath);

                HttpResponseMessage response = await httpClient.GetAsync("https://zoodex-cnn.onrender.com/predict/" + filePath);

                string responseJson = await response.Content.ReadAsStringAsync();
                Predict predict = JsonConvert.DeserializeObject<Predict>(responseJson);
                predict.Prediction = predict.Prediction.Replace("-", " ");
                using (Context)
                {
                    Reptiles reptile = Context.Reptiles.Where(a => a.Specie.Contains(predict.Prediction)).FirstOrDefault();
                    ViewBag.Reptile = reptile;
                    ViewBag.Confidence = predict.Probability + "%";
                    ViewBag.Image = "https://localhost:44378/Storage/Analysis/" + currentFile.FileName;
                }

            }
            return View();
        }
    }
}