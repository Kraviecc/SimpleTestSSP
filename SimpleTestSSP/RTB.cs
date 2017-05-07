using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SimpleTestSSP.DAL;
using SimpleTestSSP.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleTestSSP
{
    public class RTB
    {
        private readonly int maxAuctionTime = 150; //ms
        private readonly static Lazy<RTB> _instance = new Lazy<RTB>(() => new RTB(GlobalHost.ConnectionManager.GetHubContext<RTBHub>().Clients));
        public readonly ConcurrentBag<Auction> auctions = new ConcurrentBag<Auction>();
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

        public async Task<Bid> BroadcastNewAuction(Auction auction)
        {
            auction.ID = Guid.NewGuid().ToString();
            auctions.Add(auction);

            WriteLine("New auction arrived.", auction.ID);
            Clients.All.newAuction(auction);

            await Task.Delay(maxAuctionTime);
            auction.IsValid = false;
            WriteLine("Auction is no longer valid.", auction.ID);

            auction.WinningBid = calculateWinningBid(auction);

            if (auction.WinningBid != null)
            {
                WriteLine("Winning bid: " + auction.WinningBid.ClientID + " " + auction.WinningBid.Amount + "$.", auction.ID);
                Clients.Client(auction.WinningBid.ClientID).infoWinLose(new Info
                {
                    AuctionID = auction.ID,
                    IsWin = true,
                    Message = "You won this auction. Congratulations! You pay " + auction.WinningBid.Amount + "$."
                });
                Clients.AllExcept(auction.WinningBid.ClientID).infoWinLose(new Info
                {
                    AuctionID = auction.ID,
                    IsWin = false,
                    Message = "You lost this auction. Someone gave higher bid."
                });
            }
            else
                WriteLine("No bids.", auction.ID);

            return auction.WinningBid;
        }

        public IEnumerable<Auction> GetValidAuctions()
        {
            return auctions.Where(auction => auction.IsValid == true);
        }

        public void AddBid(Bid bid, string connectionID)
        {
            lock (_addBidLock)
            {
                bid.ClientID = connectionID;
                var auctionToAddBid = auctions.FirstOrDefault(auction => auction.ID == bid.AuctionID);

                if (auctionToAddBid != null && auctionToAddBid.IsValid)
                {
                    auctionToAddBid.Bids.Add(bid);

                    WriteLine("Added bid from " + bid.ClientID + ", amount: " + bid.Amount + "$.", bid.AuctionID);
                }
                else if (auctionToAddBid == null)
                {
                    WriteLine("Client " + bid.ClientID + " sent bid with incorrect auction ID.", auctionToAddBid.ID);
                    Clients.Client(bid.ClientID).infoWinLose(new Info
                    {
                        AuctionID = auctionToAddBid.ID,
                        IsWin = false,
                        Message = "You sent the bid with incorrect auction ID."
                    });
                }
                else if (!auctionToAddBid.IsValid)
                {
                    WriteLine("Client " + bid.ClientID + " sent bid too late.", auctionToAddBid.ID);
                    Clients.Client(bid.ClientID).infoWinLose(new Info
                    {
                        AuctionID = auctionToAddBid.ID,
                        IsWin = false,
                        Message = "You sent the bid too late."
                    });
                }
            }
        }

        private Bid calculateWinningBid(Auction auction) // second price auction
        {
            if (auction.Bids.Count < 2)
            {
                var winningBid = auction.Bids.FirstOrDefault();
                if (winningBid != null)
                    winningBid.Amount = 0.01;

                return winningBid;
            }
            else
            {
                var winningBid = auction.Bids.OrderByDescending(bid => bid.Amount).ElementAt(0);
                winningBid.Amount = auction.Bids.OrderByDescending(bid => bid.Amount).ElementAt(1).Amount + 0.01;
                return winningBid;
            }
        }

        private void WriteLine(string text = "", string auctionID = "")
        {
            Trace.TraceInformation("\n\t{0}\n\tAID - {1}\n\t{2}", DateTime.Now, auctionID, text);
        }
    }
}