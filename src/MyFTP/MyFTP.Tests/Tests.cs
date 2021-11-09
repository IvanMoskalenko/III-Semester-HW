using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MyFTPClient;
using MyFTPServer;
using NUnit.Framework;

namespace MyFTP.Tests
{
    //[NonParallelizable]
    public class Tests
    {
        private readonly string _path = Path.Join("..", "..", "..", "..", "MyFTP.Tests", "TestData");
        private const string Ip = "127.0.0.1";
        private const int Port = 1337;
        private readonly Client _client = new(Ip, Port);
        private readonly Server _server = new(IPAddress.Parse(Ip), Port);

        [SetUp]
        public void Setup()
        {
            _server.Start();
        }

        [Test]
        public async Task ListShouldReturnRightSizeAndItems()
        {
            var actual = await _client.List(_path);
            Assert.AreEqual(3, actual.Count);
            if (actual.Contains((Path.Join(_path, "directory"), true)) &&
                actual.Contains((Path.Join(_path, "test1.txt"), false)) &&
                actual.Contains((Path.Join(_path, "test2.txt"), false)))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task GetShouldReturnRightSizeAndDownloadFile()
        {
            var filePath = Path.Join(_path, "test1.txt");
            var newFilePath = Path.Join(_path, "test3.txt");
            var actual = await _client.Get(filePath, newFilePath);
            FileAssert.AreEqual(filePath, newFilePath);
            File.Delete(newFilePath);
            Assert.AreEqual(7, actual);
        }

        [Test]
        public void ExceptionShouldRaiseWhenFileDoesNotExist()
        {
            var filePath = Path.Join(_path, "thisFileDoesNotExist.txt");
            var newFilePath = Path.Join(_path, "test3.txt");
            Assert.Throws<AggregateException>(() => _client.Get(filePath, newFilePath).Wait());
        }

        [Test]
        public void ExceptionShouldRaiseWhenDirectoryDoesNotExist()
        {
            var path = Path.Join(_path, "DirectoryDoesNotExist");
            Assert.Throws<AggregateException>(() => _client.List(path).Wait());
        }
    }
}