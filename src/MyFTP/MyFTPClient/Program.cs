using System;
using System.Net;
using System.Threading.Tasks;

namespace MyFTPClient
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            void ShowHelp()
            {
                Console.WriteLine("Incorrect number of arguments \n" +
                                  "Please, specify arguments in {command} " +
                                  "{path} {IP} {port} {optionally: pathToSave} format.\n");
            }

            if (args.Length != 4 && args.Length != 5)
            {
                ShowHelp();
                return;
            }

            if (!IPAddress.TryParse(args[2], out _))
            {
                ShowHelp();
                Console.WriteLine("Incorrect IP");
                return;
            }

            if (!int.TryParse(args[3], out _))
            {
                ShowHelp();
                Console.WriteLine("Incorrect port");
                return;
            }

            var client = new Client(args[2], Convert.ToInt32(args[3]));

            switch (args[0])
            {
                case "1":
                    var response = await client.List(args[1]);
                    Console.WriteLine(response.Count);
                    foreach (var (path, isDir) in response)
                    {
                        Console.WriteLine($"{path} {isDir}");
                    }

                    break;
                case "2":
                    if (args.Length != 5)
                    {
                        ShowHelp();
                    }

                    var size = await client.Get(args[1], args[4]);
                    Console.WriteLine(size);
                    break;
                default:
                    Console.WriteLine("Incorrect command");
                    return;
            }
        }
    }
}