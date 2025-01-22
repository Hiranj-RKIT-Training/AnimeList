using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.poco
{
    public class ANI01
    {
        // Anime Id
        [PrimaryKey, AutoIncrement]
        public int I01F01 { get; set; }
        // Anime Title
        [ValidateNotNull]
        public string I01F02 { get; set; }
        // No of Seasons
        public int I01F03 { get; set; }
        // No of Episodes
        public int I01F04 { get; set; }
        // Release Year
        public int I01F05 { get; set; }
    }
}