using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zoodex.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public int PostedById { get; set; }
        public int? ProfileImageId { get; set; }
        public string PostedBy { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
    }
}