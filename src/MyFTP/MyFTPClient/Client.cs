using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyFTPClient
{
    public static class Client
    {
        public static async Task<string> Run(string command, string path, string ip, int port, string pathToSave = "")
        {
            using var client = new TcpClient(ip, port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(command + " " + path);
            await writer.FlushAsync();
            var reader = new StreamReader(stream);
            switch (command)
            {
                case "1":
                    var data = await reader.ReadToEndAsync();
                    return data;
                case "2":
                    var size = await reader.ReadLineAsync();
                    var content = await reader.ReadToEndAsync();
                    await File.WriteAllBytesAsync(pathToSave, Convert.FromBase64String(content));
                    return size;
                default:
                    return "Incorrect command";
            }
        }
    }
}