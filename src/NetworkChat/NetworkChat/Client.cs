using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkChat
{
    /// <summary>
    /// Implementation of client
    /// </summary>
    public class Client
    {
        private readonly TcpClient _client;
        private NetworkStream _stream;
        private readonly CancellationTokenSource _cancellationTokenSource;
        
        public Client(int port, string ip)
        {
            _client = new TcpClient(ip, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Starts client
        /// </summary>
        public async Task Start()
        {
            await using (_stream = _client.GetStream())
            {
                Receive();
                await Send();
            }
        }
        
        /// <summary>
        /// Receives messages
        /// </summary>
        private void Receive()
        {
            Task.Run(async () =>
            {
                using var reader = new StreamReader(_stream);
                var received = await reader.ReadLineAsync();
                while (received != "exit")
                {
                    Console.WriteLine(received);
                    received = await reader.ReadLineAsync();
                }

                _cancellationTokenSource.Cancel();
                _client.Close();
            });
        }
        
        /// <summary>
        /// Sends messages
        /// </summary>
        /// <returns>Task</returns>
        private Task Send()
        {
            return Task.Run(async () =>
            {
                await using var writer = new StreamWriter(_stream) {AutoFlush = true};
                var tasks = new List<Task>();
                var message = Console.ReadLine();
                while (message != "exit")
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    tasks.Add(writer.WriteLineAsync(message));
                    message = Console.ReadLine();
                }

                Task.WaitAll(tasks.ToArray());
                _client.Close();
            });
        }
    }
}