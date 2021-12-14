using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator
{
    /// <summary>
    /// Implementation of checksum calculator
    /// </summary>
    public static class ChecksumCalculator
    {
        /// <summary>
        /// Calculates checksum of directory or file
        /// </summary>
        /// <param name="path">Path to directory of file where checksum must be calculated</param>
        /// <param name="multiThreaded">Indicates the need to parallelize the process</param>
        /// <returns>Checksum in byte[] format</returns>
        /// <exception cref="ArgumentException">Specified path doesn't exist</exception>
        public static byte[] Calculate(string path, bool multiThreaded)
        {
            if (File.Exists(path))
            {
                return CalculateFileHash(path);
            }

            if (!Directory.Exists(path)) throw new ArgumentException("Invalid path");

            return multiThreaded ? ParallelCalculateDirectoryHash(path) : CalculateDirectoryHash(path);
        }

        /// <summary>
        /// Calculates checksum of file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Checksum in byte[] format</returns>
        private static byte[] CalculateFileHash(string path)
        {
            using var file = File.OpenRead(path);
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(file);
            return hash;
        }

        /// <summary>
        /// Gets all subdirectories and files in directory
        /// </summary>
        /// <param name="path">Path to directory</param>
        /// <returns>IEnumerable of items</returns>
        private static IEnumerable<string> GetAllDirectoryItems(string path)
        {
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            var items = files.Concat(directories);
            return items;
        }

        /// <summary>
        /// Calculates directory checksum
        /// </summary>
        /// <param name="path">Path to directory</param>
        /// <returns>Checksum in byte[] format</returns>
        private static byte[] CalculateDirectoryHash(string path)
        {
            var items = GetAllDirectoryItems(path);
            var dictionary = new Dictionary<string, byte[]>();
            foreach (var item in items)
            {
                byte[] hash = File.Exists(item) ? CalculateFileHash(item) : CalculateDirectoryHash(item);
                dictionary[item] = hash;
            }

            var sorted = dictionary.OrderBy(x => x.Key);
            var bytes = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)!);
            foreach (var (_, value) in sorted)
            {
                bytes = bytes.Concat(value).ToArray();
            }

            using var md5 = MD5.Create();
            var resultHash = md5.ComputeHash(bytes);
            return resultHash;
        }

        /// <summary>
        /// Calculates directory checksum in parallel
        /// </summary>
        /// <param name="path">Path to directory</param>
        /// <returns>Checksum in byte[] format</returns>
        private static byte[] ParallelCalculateDirectoryHash(string path)
        {
            var items = GetAllDirectoryItems(path);
            var dictionary = new ConcurrentDictionary<string, byte[]>();
            Parallel.ForEach(items, item =>
            {
                byte[] hash = File.Exists(item) ? CalculateFileHash(item) : ParallelCalculateDirectoryHash(item);
                dictionary[item] = hash;
            });
            var sorted = dictionary.OrderBy(x => x.Key);
            var bytes = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)!);
            foreach (var (_, value) in sorted)
            {
                bytes = bytes.Concat(value).ToArray();
            }

            using var md5 = MD5.Create();
            var resultHash = md5.ComputeHash(bytes);
            return resultHash;
        }
    }
}