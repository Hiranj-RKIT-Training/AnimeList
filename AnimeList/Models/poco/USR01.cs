using AnimeList.Models.enums;
using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.poco
{
    public class USR01
    {
           // User Id
           [PrimaryKey , AutoIncrement]
           public int R01F01 { get; set; }
           // user Email
           [Unique, ValidateEmail, ValidateNotNull]
           public string R01F02 { get; set; }
           // Password
           [ValidateNotNull]
           public string R01F03 { get; set; }
           // Role
           //[Default(1)]
           public enmRole R01F04 { get; set; }
           // First name
           public string R01F05 { get; set; }
           // Last name
           public string R01F06 { get; set; }
           // Age
           public int R01F07 { get; set; }
    }
}