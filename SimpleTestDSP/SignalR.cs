using Microsoft.AspNet.SignalR.Client;
using SimpleTestSSP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTestDSP
{
    public class SignalR
    {
        readonly Random random = new Random();
        readonly object randomLock = new object();

        string SSPUrl = "http://localhost:62664/";
        string connectionID = string.Empty;
        bool isError = false;
        IHubProxy myHub = null;
        string ad = string.Empty;

        public SignalR()
        {
        }

        public void Run(string ad)
        {
            this.ad = ad;
            var connection = new HubConnection(SSPUrl);
            myHub = connection.CreateHubProxy("RTBHub");

            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    isError = true;
                    WriteLine(false, "There was an error opening the connection: " + task.Exception.Message + "\n\nPress any key to close application.");
                }
                else
                {
                    connectionID = connection.ConnectionId;
                    WriteLine(false, "Connected with " + SSPUrl + "successfully.");
                }

            }).Wait();

            if (!isError)
            {
                myHub.On<Auction>("newAuction", auction =>
                {
                    WriteLine(false, "New auction arrived.", auction.ID);
                    addBid(auction);
                });

                myHub.On<Info>("infoWinLose", info =>
                {
                    WriteLine(false, (info.IsWin ? "WIN" : "LOSE") + ". " + info.Message, info.AuctionID);
                });

                myHub.Invoke<IEnumerable<Auction>>("GetValidAuctions").ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        WriteLine(false, "There was an error calling GetValidAuctions: " + task.Exception.Message + ".");
                    }
                    else
                    {
                        addBid(task.Result);
                    }
                });
            }

            Console.Read();
            connection.Stop();
        }

        public void WriteLine(bool initial = true, string text = "", string auctionID = "")
        {
            if (!initial)
                Console.WriteLine("\n\n{0}\n\tCID - {1}\n\tAID - {2}\n\t{3}", DateTime.Now, connectionID, auctionID, text);
            else
                Console.WriteLine("DSP {0} is initializing...\n\nAfter successful init, in case you want to end application, press any key.\n\nAbbrv.:\n\tCID - Client ID\n\tAID - Auction ID", text);
        }

        private void addBid(Auction auction)
        {
            Bid bid = new Bid { AuctionID = auction.ID, Amount = Math.Round(getRandom(), 2), Ad = ad };

            WriteLine(false, "Sent Bid: " + bid.Amount + "$", auction.ID);

            myHub.Invoke<Bid>("AddBid", bid);
        }

        private void addBid(IEnumerable<Auction> auctions)
        {
            if (auctions.Count() > 0)
                WriteLine(false, "Few auctions are open. Sending bids...");
            foreach (var auction in auctions)
                addBid(auction);
        }

        private double getRandom()
        {
            lock (randomLock)
            {
                return random.NextDouble() + Convert.ToDouble(random.Next(0, 5)); // 0.0 - 6.0
            }
        }
    }
}
