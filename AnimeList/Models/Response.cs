using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models
{
    public class Response
    {
        public dynamic data { get; set; }
        public bool IsError { get; set; }
        public string message { get; set; }
    }
}