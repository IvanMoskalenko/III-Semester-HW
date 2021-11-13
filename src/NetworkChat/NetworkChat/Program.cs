using System;
using System.Threading.Tasks;

namespace NetworkChat
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length is 0 or > 2)
            {
                Console.WriteLine("Number of arguments is invalid." +
                                  "\nPlease, specify one argument if you want to start server " +
                                  "and two arguments if you want start client.");
                return;
            }

            var isValidPort = int.TryParse(args[0], out var port);
            if (!isValidPort)
            {
                Console.WriteLine("Sorry, but port is invalid.");
                return;
            }

            if (args.Length == 1)
            {
                var server = new Server(port);
                await server.Start();
            }
            else
            {
                var client = new Client(port, args[1]);
                await client.Start();
            }
        }
    }
}