using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleTestSSP.DAL
{
    public class Auction
    {
        public string ID { get; set; } = string.Empty;

        public bool IsValid { get; set; } = true;

        public Bid WinningBid { get; internal set; } = null;

        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}