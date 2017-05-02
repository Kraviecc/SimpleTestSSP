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

        static void Main(string[] args)
        {
            Thread.Sleep(5000);

            var connection = new HubConnection("http://localhost:62664/");
            var myHub = connection.CreateHubProxy("RTBHub");

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
                myHub.Invoke<IEnumerable<Auction>>("GetValidAuctions").ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error calling send: {0}",
                                          task.Exception.GetBaseException());
                    }
                    else
                    {
                        Console.WriteLine(task.Result.FirstOrDefault()?.ID);
                    }
                });

                connection.Stop();
            }

            Console.Read();
        }

        static void addBid(Auction auction)
        {

        }
    }
}
