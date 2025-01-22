using AnimeList.Models.enums;
using ServiceStack.DataAnnotations;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.dto
{
    public class DTOUSR01
    {
        // User Id
        [Required]
        public int R01F01 { get; set; }
        // user Email
        public string R01F02 { get; set; }
        // Password
        public string R01F03 { get; set; }
        // Role
        public enmRole R01F04 { get; set; }
        // First name
        public string R01F05 { get; set; }
        // Last name
        public string R01F06 { get; set; }
        // Age
        public int R01F07 { get; set; }
    }
}