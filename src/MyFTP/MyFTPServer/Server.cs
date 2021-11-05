using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyFTPServer
{
    public static class Server
    {
        private static async Task List(TextWriter writer, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                await writer.WriteAsync("-1");
                return;
            }

            var files = Directory.GetFiles(directoryPath);
            var directories = Directory.GetDirectories(directoryPath);
            var size = files.Length + directories.Length;
            var result = size + "\n";
            result = directories.Aggregate(result, (current, directory) => current + directory + " true\n");
            result = files.Aggregate(result, (current, file) => current + file + " false\n");
            await writer.WriteAsync(result);
        }

        private static async Task Get(TextWriter writer, string path)
        {
            if (!File.Exists(path))
            {
                await writer.WriteAsync("-1");
                return;
            }

            var size = new FileInfo(path).Length;
            await writer.WriteLineAsync(size.ToString());

            var content = await File.ReadAllBytesAsync(path);
            await writer.WriteAsync(Convert.ToBase64String(content));
        }

        private static async Task Work(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var data = (await reader.ReadLineAsync())?.Split(' ');
            var writer = new StreamWriter(stream);
            if (data != null)
            {
                var command = data[0];
                var path = data[1];
                switch (command)
                {
                    case "1":
                        await List(writer, path);
                        break;
                    case "2":
                        await Get(writer, path);
                        break;
                    default:
                        await writer.WriteAsync("Incorrect command");
                        break;
                }
            }

            await writer.FlushAsync();
            socket.Close();
        }

        public static async Task Start(IPAddress address, int port)
        {
            var listener = new TcpListener(address, port);
            Console.WriteLine($"Started working at {address}:{port}");
            listener.Start();
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                await Task.Run(() => Work(socket));
            }
        }
    }
}