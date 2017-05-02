using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SimpleTestSSP.DAL;
using SimpleTestSSP.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimpleTestSSP
{
    public class RTB
    {
        private readonly static Lazy<RTB> _instance = new Lazy<RTB>(() => new RTB(GlobalHost.ConnectionManager.GetHubContext<RTBHub>().Clients));
        private readonly ConcurrentBag<Auction> _auctions = new ConcurrentBag<Auction>();
        private readonly object _addBidLock = new object();
        private IHubConnectionContext<dynamic> Clients { get; set; }

        public static RTB Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private RTB(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public void BroadcastNewAuction(Auction auction)
        {
            _auctions.Add(auction);

            Clients.All.newAuction(auction);
        }

        public IEnumerable<Auction> GetValidAuctions()
        {
            return _auctions.Where(auction => auction.IsValid == true);
        }

        public Result AddBid(Bid bid, string connectionID)
        {
            Result result = new Result();
            lock (_addBidLock)
            {
                var auctionToAddBid = _auctions.FirstOrDefault(auction => auction.ID == bid.AuctionID);
                
                if (auctionToAddBid != null)
                {
                    bid.ClientID = connectionID;
                    auctionToAddBid.Bids.Add(bid);

                    result.Message = "Bid added to auction = " + bid.AuctionID;
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Incorrect field value: '" + nameof(Bid.AuctionID) + "'";
                }

                return result;
            }
        }
    }
}