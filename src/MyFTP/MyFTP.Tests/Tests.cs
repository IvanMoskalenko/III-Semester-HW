using System.IO;
using System.Net;
using System.Threading.Tasks;
using MyFTPClient;
using MyFTPServer;
using NUnit.Framework;

namespace MyFTP.Tests
{
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
            var expected = "5\n" 
                                    + Path.Join(_path, "Empty2") + " true\n"
                                    + Path.Join(_path, "Empty1") + " true\n"
                                    + Path.Join(_path, "empty1") + " false\n"
                                    + Path.Join(_path, "empty3") + " false\n"
                                    + Path.Join(_path, "empty2") + " false\n";
            Assert.AreEqual(expected, actual);
        }
        // [Test]
        // public async Task GetShouldReturnRightSizeAndDownloadFile()
        // {
        //     var actual = await Client.Run("2", _path + "/empty1", Ip, Port, _path + "/test");
        //     FileAssert.AreEqual(_path + "/empty1", _path + "/test");
        //     File.Delete(_path + "/test");
        //     Assert.AreEqual("7", actual);
        // }
    }
}