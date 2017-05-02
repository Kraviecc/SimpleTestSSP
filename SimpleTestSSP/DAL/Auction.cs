using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleTestSSP.DAL
{
    public class Auction
    {
        public int ID { get; set; } = 0;

        public bool IsValid { get; set; } = false;

        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}