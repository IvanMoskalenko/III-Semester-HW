using System;
using System.Net;
using System.Threading.Tasks;

namespace MyFTPServer
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await Server.Start(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
        }
    }
}