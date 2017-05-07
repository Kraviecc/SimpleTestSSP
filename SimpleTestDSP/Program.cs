using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTestDSP
{
    class Program
    {
        static string ad = string.Empty;
        static int number = 0;

        static void Main(string[] args)
        {
            number = getDSPNumber();

            SignalR signalR = new SignalR();
            signalR.WriteLine(text: number.ToString());

            ad = getAd(number);
            signalR.Run(ad);
        }

        static int getDSPNumber()
        {
            return System.Diagnostics.Process
                    .GetProcessesByName(Path
                    .GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() + 1;
        }

        static string getAd(int number)
        {
            string ad = string.Empty;

            DirectoryInfo dInfo = new DirectoryInfo(Environment.CurrentDirectory);
            var ads = dInfo.GetFiles(number + ".*");
            var alternativeAd = dInfo.GetFiles("1.*");

            if (ads.Length > 0)
                ad = Convert.ToBase64String(File.ReadAllBytes(ads.FirstOrDefault().FullName));
            else if (alternativeAd.Length > 0)
                ad = Convert.ToBase64String(File.ReadAllBytes(alternativeAd.FirstOrDefault().FullName));
            else
            {
                Console.WriteLine("\n\nNo ads in app directory. Shutting down...");
                Console.Read();
                Environment.Exit(0);
            }
            return ad;
        }
    }
}
