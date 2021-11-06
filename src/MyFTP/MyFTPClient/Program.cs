using System;
using System.Net;
using System.Threading.Tasks;

namespace MyFTPClient
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.Write("Enter IP: ");
            var ip = Console.ReadLine();
            if (!IPAddress.TryParse(ip, out _))
            {
                Console.WriteLine("Incorrect IP");
                return;
            }

            Console.Write("Enter port: ");
            int port;
            try
            {
                port = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Incorrect port");
                return;
            }

            switch (args[0])
            {
                case "1":
                    var response = await Client.Run("1", args[1], ip, port);
                    Console.WriteLine(response);
                    break;
                case "2":
                    Console.WriteLine("Enter path to save file: ");
                    var pathToSave = Console.ReadLine();
                    var size = await Client.Run("2", args[1], ip, port, pathToSave);
                    Console.WriteLine(size);
                    break;
                default:
                    Console.WriteLine("Incorrect command");
                    return;
            }
        }
    }
}