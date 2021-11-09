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

            var server = new Server(IPAddress.Parse(args[0]), Convert.ToInt32(args[1])); 
            
            try
            {
                await server.Start();
            }
            catch (FormatException)
            {
                Console.WriteLine("Incorrect port");
            }
        }
    }
}