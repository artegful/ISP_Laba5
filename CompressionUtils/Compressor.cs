using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferProtocolLibrary.Compression
{
    public static class Compressor
    {
        public async static Task CompressAsync(string sourceFile, string compressedFile)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                            await Task.Run(() => sourceStream.CopyTo(compressionStream));
                    }
                }
            }
        }

        public async static Task DecompressAsync(string compressedFile, string targetFile)
        {
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(targetFile))
                {
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        await Task.Run(() => decompressionStream.CopyTo(targetStream));
                    }
                }
            }
        }
    }
}
