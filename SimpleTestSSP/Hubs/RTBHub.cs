using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SimpleTestSSP.DAL;

namespace SimpleTestSSP.Hubs
{
    [HubName("RTBHub")]
    public class RTBHub : Hub
    {
        private readonly RTB _RTB;

        public RTBHub() : this(RTB.Instance) { }
        public RTBHub(RTB rtb)
        {
            _RTB = rtb;
        }

        public IEnumerable<Auction> GetValidAuctions()
        {
            return _RTB.GetValidAuctions();
        }

        public Result AddBid(Bid bid)
        {
            return _RTB.AddBid(bid, Context.ConnectionId);
        }
    }
}