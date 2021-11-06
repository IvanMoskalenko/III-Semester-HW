using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MyFTPClient;
using MyFTPServer;
using NUnit.Framework;

namespace MyFTP.Tests
{
    [NonParallelizable]
    public class Tests
    {
        private readonly string _path = Path.Join("..", "..", "..", "..", "MyFTP.Tests", "TestData");
        private const string Ip = "127.0.0.1";
        private const int Port = 1337;
        
        [SetUp]
        public void Setup()
        {

            Server.Start(IPAddress.Parse(Ip), Port);
        }
        
        [Test]
        public async Task ListShouldReturnRightSizeAndItems()
        {
            var actual = await Client.Run("1", _path, Ip, Port);
            var expected = "3\n"
                           + Path.Join(_path, "directory") + " true\n"
                           + Path.Join(_path, "test1.txt") + " false\n"
                           + Path.Join(_path, "test2.txt") + " false\n";
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task GetShouldReturnRightSizeAndDownloadFile()
        {
            var filePath = Path.Join(_path, "test1.txt");
            var newFilePath = Path.Join(_path, "test3.txt");
            var actual = await Client.Run("2", filePath, Ip, Port, newFilePath);
            FileAssert.AreEqual(filePath, newFilePath);
            File.Delete(newFilePath);
            Assert.AreEqual("7", actual);
        }

        [Test]
        public void ExceptionShouldRaiseWhenFileDoesNotExist()
        {
            var filePath = Path.Join(_path, "thisFileDoesNotExist.txt");
            var newFilePath = Path.Join(_path, "test3.txt");
            Assert.Throws<AggregateException>( () => Client.Run("2", filePath, Ip, Port, newFilePath).Wait());
        }
        
        [Test]
        public void ExceptionShouldRaiseWhenDirectoryDoesNotExist()
        {
            var path = Path.Join(_path, "DirectoryDoesNotExist");
            Assert.Throws<AggregateException>( () => Client.Run("1", path, Ip, Port).Wait());
        }
    }
}