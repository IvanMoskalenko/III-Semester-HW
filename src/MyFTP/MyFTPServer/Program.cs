using System;
using System.Net;
using System.Threading.Tasks;

namespace MyFTPServer
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect number of arguments \n" +
                                  "Please, specify arguments in {IP} {port} format.");
                return;
            }

            if (!IPAddress.TryParse(args[0], out _))
            {
                Console.WriteLine("Incorrect IP");
                return;
            }

            if (!int.TryParse(args[1], out _))
            {
                Console.WriteLine("Incorrect port");
                return;
            }

            var server = new Server(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
            await server.Start();
        }
    }
}