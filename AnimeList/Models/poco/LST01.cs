using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeList.Models.poco
{
    public class LST01
    {
        // List Id
        [PrimaryKey, AutoIncrement]
        public int T01F01 { get; set; }
        // user id
        [ValidateNotNull , ForeignKey(typeof(USR01))]
        public int T01F02 { get; set; }
        // list Name
        public string T01F03 { get; set; }
    }
}