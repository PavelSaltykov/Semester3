using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    public static class CheckSum
    {
        private static byte[] ComputeFile(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            return md5.ComputeHash(stream);
        }

        public static byte[] ComputeSingleThreaded(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                throw new InvalidOperationException("Incorrect path to directory or file.");

            if (Directory.Exists(path))
            {
                var systemEntries = Directory.EnumerateFileSystemEntries(path).OrderBy(entry => entry);
                var result = Encoding.UTF8.GetBytes(new DirectoryInfo(path).Name);
                foreach (var item in systemEntries)
                {
                    result = result.Concat(ComputeSingleThreaded(item)).ToArray();
                }
                return result;
            }

            return ComputeFile(path);
        }

        public static async Task<byte[]> ComputeMultiThreaded(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                throw new InvalidOperationException("Incorrect path to directory or file.");

            if (Directory.Exists(path))
            {
                var systemEntries = Directory.EnumerateFileSystemEntries(path).OrderBy(entry => entry);
                var result = Encoding.UTF8.GetBytes(new DirectoryInfo(path).Name);
                foreach (var item in systemEntries)
                {
                    result = result.Concat(await ComputeMultiThreaded(item)).ToArray();
                }
                return result;
            }

            return ComputeFile(path);
        }
    }
}
