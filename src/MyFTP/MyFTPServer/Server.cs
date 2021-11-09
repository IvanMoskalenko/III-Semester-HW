using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyFTPServer
{
    /// <summary>
    /// Implementation of server
    /// </summary>
    public class Server
    {
        private readonly IPAddress _ip;
        private readonly int _port;
        private readonly CancellationTokenSource _tokenSource;
        private readonly List<Task> _clients;

        public Server(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            _tokenSource = new CancellationTokenSource();
            _clients = new List<Task>();
        }

        /// <summary>
        /// Executes "Get" command
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
            var result = new StringBuilder(size.ToString());
            foreach (var directory in directories)
            {
                result.AppendFormat($" {directory} true");
            }

            foreach (var file in files)
            {
                result.AppendFormat($" {file} false");
            }

            await writer.WriteAsync(result.ToString());
        }

        /// <summary>
        /// Executes "Get" command
        /// </summary>
        /// <param name="writer">StreamWriter for writing response</param>
        /// <param name="path">Path to get file</param>
        private static async Task Get(StreamWriter writer, string path)
        {
            if (!File.Exists(path))
            {
                await writer.WriteAsync("-1");
                return;
            }

            var size = new FileInfo(path).Length;
            await writer.WriteAsync($"{size} ");
            await using var fileStream = File.OpenRead(path);
            await fileStream.CopyToAsync(writer.BaseStream);
        }
        
        /// <summary>
        /// Listens client
        /// </summary>
        /// <param name="socket">Socket for listening clients</param>
        private static async Task Work(Socket socket)
        {
            using (socket)
            {
                await using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                var data = (await reader.ReadLineAsync())?.Split(' ');
                var writer = new StreamWriter(stream) {AutoFlush = true};
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
            }

            socket.Close();
        }
        
        /// <summary>
        /// Stops server
        /// </summary>
        public void Stop() => _tokenSource.Cancel();
        
        /// <summary>
        /// Starts server
        /// </summary>
        public async Task Start()
        {
            var listener = new TcpListener(_ip, _port);
            Console.WriteLine($"Started working at {_ip}:{_port}");
            listener.Start();
            while (!_tokenSource.Token.IsCancellationRequested)
            {
                var socket = await listener.AcceptSocketAsync();
                var client = Task.Run(() => Work(socket));
                _clients.Add(client);
            }

            Task.WaitAll(_clients.ToArray());
        }
    }
}