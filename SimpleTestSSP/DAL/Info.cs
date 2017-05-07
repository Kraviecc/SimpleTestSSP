using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleTestSSP.DAL
{
    public class Info
    {
        public string AuctionID { get; set; } = string.Empty;

        public bool IsWin { get; set; } = false;

        public string Message { get; set; } = string.Empty;
    }
}