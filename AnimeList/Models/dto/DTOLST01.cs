using AnimeList.Models.poco;
using ServiceStack.DataAnnotations;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.dto
{
    public class DTOLST01
    {
        public int T01F01 { get; set; }
        // user id
        public int T01F02 { get; set; }
        // list Name
        public string T01F03 { get; set; }
    }
}