using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyFTPClient
{
    /// <summary>
    /// Implementation of client
    /// </summary>
    public class Client
    {
        private readonly string _ip;
        private readonly int _port;

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public async Task<List<(string, bool)>> List(string command, string path)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_ip, _port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) { AutoFlush = true };
            await writer.WriteLineAsync(command + " " + path);
            var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            if (data == "-1")
            {
                throw new DirectoryNotFoundException();
            }
            var splitData = data.Split(' ');
            var result = new List<(string, bool)>();
            for (var i = 1; i < splitData.Length; i += 2)
            {
                result.Add((splitData[i], Convert.ToBoolean(splitData[i + 1])));
            }

            return result;
        }
        
        public async Task<int> Get(string command, string path, string pathToSave)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_ip, _port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) { AutoFlush = true };
            await writer.WriteLineAsync(command + " " + path);
            var reader = new StreamReader(stream);
            var size = new StringBuilder();
            while (reader.Peek() != ' ')
            {
                if (reader.Peek() == '-')
                {
                    throw new FileNotFoundException();
                }
                size.Append((char)reader.Read());
            }
            reader.Read();
            await using var fileStream = File.Create(pathToSave);
            await stream.CopyToAsync(fileStream);
            return Convert.ToInt32(size.ToString());
        }
        
    }
}