using AnimeList.Models.enums;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.poco
{
    public class ANL01
    {
        // List Id
        [PrimaryKey , ForeignKey(typeof(LST01)) ]
        public int L01F01 { get; set; }
        // Anime Id
        [PrimaryKey, ForeignKey(typeof(ANI01))]
        public int L01F02 { get; set; }
        // Status
        public enmStatus L01F03 { get; set; }
    }
}