using AnimeList.Models.enums;
using AnimeList.Models.poco;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.dto
{
    public class DTOANL01
    {
        // List I
        public int L01F01 { get; set; }
        // Anime Id
        public int L01F02 { get; set; }
        // Status
        public enmStatus L01F03 { get; set; }
    }
}