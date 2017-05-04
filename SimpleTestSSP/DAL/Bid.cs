using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleTestSSP.DAL
{
    public class Bid
    {
        public string AuctionID { get; set; } = "";

        public string ClientID { get; internal set; } = string.Empty;

        public double Amount { get; set; } = 0;
    }
}