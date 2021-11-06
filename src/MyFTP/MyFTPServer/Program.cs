using System;
using System.Net;
using System.Threading.Tasks;

namespace MyFTPServer
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (!IPAddress.TryParse(args[0], out _))
            {
                Console.WriteLine("Incorrect IP");
                return;
            }

            try
            {
                await Server.Start(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
            }
            catch (FormatException)
            {
                Console.WriteLine("Incorrect port");
            }
        }
    }
}