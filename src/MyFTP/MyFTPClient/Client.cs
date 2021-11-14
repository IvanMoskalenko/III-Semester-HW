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

        /// <summary>
        /// Sends request for getting list of files and directories on server.
        /// </summary>
        /// <param name="path">Directory path for getting list</param>
        /// <returns>List of (string, bool), where string is path to directory/file,
        /// bool is flag that true for directories</returns>
        /// <exception cref="DirectoryNotFoundException">Specified directory doesn't exist</exception>
        public async Task<List<(string, bool)>> List(string path)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_ip, _port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) {AutoFlush = true};
            await writer.WriteLineAsync("1" + " " + path);
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

        /// <summary>
        /// Sends request for downloading file and getting it's size
        /// </summary>
        /// <param name="path">Path to get file</param>
        /// <param name="pathToSave">Path to save file</param>
        /// <returns>Size of file</returns>
        /// <exception cref="FileNotFoundException">File doesn't exist on server</exception>
        public async Task<int> Get(string path, string pathToSave)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_ip, _port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) {AutoFlush = true};
            await writer.WriteLineAsync("2" + " " + path);
            var reader = new StreamReader(stream);
            var size = new StringBuilder();
            while (reader.Peek() != ' ')
            {
                if (reader.Peek() == '-')
                {
                    throw new FileNotFoundException();
                }

                size.Append((char) reader.Read());
            }

            reader.Read();
            await using var fileStream = File.Create(pathToSave);
            await stream.CopyToAsync(fileStream);
            return Convert.ToInt32(size.ToString());
        }
    }
}