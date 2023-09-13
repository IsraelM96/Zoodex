using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zoodex.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string Hostname { get; set; }
        public string IP { get; set; }
        public DateTime Date { get; set; }
    }
}