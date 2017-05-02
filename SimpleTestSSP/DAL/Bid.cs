using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleTestSSP.DAL
{
    public class Bid
    {
        public int AuctionID { get; set; } = 0;

        public string ClientID { get; internal set; } = string.Empty;
    }
}