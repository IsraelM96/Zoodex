using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zoodex.Controllers;
using Zoodex.Models;

namespace Zoodex.Functions
{
    public class ImageHandler
    {
        public static string GetProfilePicture(int? Id)
        {
            string url = "/Resources/Images/default.png";

            using (Zoodex_Entities Context = new Zoodex_Entities())
            {
                Files file = Context.Files.Where(a => a.PKFileID == Id && a.Deleted == false).FirstOrDefault();
                if (file != null)
                {
                    url = file.Path;
                }
            }

            return url;
        }
    }
}