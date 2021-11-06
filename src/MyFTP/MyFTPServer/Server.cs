using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyFTPServer
{
    /// <summary>
    /// Implementation of server
    /// </summary>
    public static class Server
    {
        /// <summary>
        /// Executes "List" command
        /// </summary>
        /// <param name="writer">TextWriter for writing response</param>
        /// <param name="directoryPath">Directory for getting list</param>
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
        
        /// <summary>
        /// Executes "Get" command
        /// </summary>
        /// <param name="writer">TextWriter for writing response</param>
        /// <param name="path">Path to get file</param>
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

        /// <summary>
        /// Listens clients
        /// </summary>
        /// <param name="socket">Socket for listening clients</param>
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
        
        /// <summary>
        /// Starts server
        /// </summary>
        /// <param name="address">IP address to get started</param>
        /// <param name="port">Port to get started</param>
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