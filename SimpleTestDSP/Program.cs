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
    class Program
    {
        static bool isError = false;
        static IHubProxy myHub = null;

        static void Main(string[] args)
        {
            var connection = new HubConnection("http://localhost:62664/");
            myHub = connection.CreateHubProxy("RTBHub");

            connection.Start().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                    isError = true;
                }
                else
                {
                    Console.WriteLine("Connected");
                }

            }).Wait();

            if (!isError)
            {
                myHub.On<Auction>("newAuction", auction => {
                    Console.WriteLine("New Auction arrived: " + auction.ID);
                    addBid(auction);
                });

                myHub.On<string>("infoWinLose", info => {
                    Console.WriteLine("Info: " + info);
                });

                //myHub.Invoke<IEnumerable<Auction>>("GetValidAuctions").ContinueWith(task =>
                //{
                //    if (task.IsFaulted)
                //    {
                //        Console.WriteLine("There was an error calling send: {0}",
                //                          task.Exception.GetBaseException());
                //    }
                //    else
                //    {
                //        addBid(task.Result);
                //    }
                //});
            }

            Console.Read();
            connection.Stop();
        }

        static void addBid(Auction auction)
        {
            Bid bid = new Bid { AuctionID = auction.ID, Amount = 33 };

            myHub.Invoke<Bid>("AddBid", bid);
        }

        static void addBid(IEnumerable<Auction> auctions)
        {
            foreach (var auction in auctions)
                addBid(auction);
        }
    }
}
