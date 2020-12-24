using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace TransferProtocolLibrary.Compression
{
    public static class Encriptor
    {
        private static CspParameters cspp = new CspParameters();
        private static RSACryptoServiceProvider rsa;

        private static string keyName = "Key01";

        private static readonly string extension = ".enc";

        public static void SetKeys(string key)
        {
            keyName = key;
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
        }

        public async static Task EncryptFileAsync(string filePath, string destinationFile)
        {
            Aes aes = Aes.Create();
            ICryptoTransform transform = aes.CreateEncryptor();

            byte[] keyEncrypted = rsa.Encrypt(aes.Key, false);

            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            int lKey = keyEncrypted.Length;
            LenK = BitConverter.GetBytes(lKey);
            int lIV = aes.IV.Length;
            LenIV = BitConverter.GetBytes(lIV);

            string outputFilePath = Path.ChangeExtension(destinationFile, extension);

            using (FileStream outFs = new FileStream(outputFilePath, FileMode.Create))
            {

                await Task.Run(() => outFs.Write(LenK, 0, 4));
                await Task.Run(() => outFs.Write(LenIV, 0, 4));
                await Task.Run(() => outFs.Write(keyEncrypted, 0, lKey));
                await Task.Run(() => outFs.Write(aes.IV, 0, lIV));

                using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {
                    int count = 0;
                    int offset = 0;

                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;

                    using (FileStream inFs = new FileStream(filePath, FileMode.Open))
                    {
                        do
                        {
                            await Task.Run(() => count = inFs.Read(data, 0, blockSizeBytes));
                            offset += count;
                            await Task.Run(() => outStreamEncrypted.Write(data, 0, count));
                            bytesRead += blockSizeBytes;
                        }
                        while (count > 0);
                        inFs.Close();
                    }
                    outStreamEncrypted.FlushFinalBlock();
                    outStreamEncrypted.Close();
                }
                outFs.Close();
            }
        }

        public async static Task DecryptFileAsync(string inFile, string destinationFile)
        {

            Aes aes = Aes.Create();

            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            inFile = Path.ChangeExtension(inFile, extension);

            using (FileStream inFs = new FileStream(inFile, FileMode.Open))
            {

                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(LenK, 0, 3);
                inFs.Seek(4, SeekOrigin.Begin);
                inFs.Read(LenIV, 0, 3);

                int lenK = BitConverter.ToInt32(LenK, 0);
                int lenIV = BitConverter.ToInt32(LenIV, 0);

                int startC = lenK + lenIV + 8;
                int lenC = (int)inFs.Length - startC;

                byte[] KeyEncrypted = new byte[lenK];
                byte[] IV = new byte[lenIV];

                inFs.Seek(8, SeekOrigin.Begin);
                inFs.Read(KeyEncrypted, 0, lenK);
                inFs.Seek(8 + lenK, SeekOrigin.Begin);
                inFs.Read(IV, 0, lenIV);

                byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

                ICryptoTransform transform = aes.CreateDecryptor(KeyDecrypted, IV);

                using (FileStream outFs = new FileStream(destinationFile, FileMode.Create))
                {

                    int count = 0;
                    int offset = 0;

                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];

                    inFs.Seek(startC, SeekOrigin.Begin);
                    using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                    {
                        do
                        {
                            await Task.Run(() => count = inFs.Read(data, 0, blockSizeBytes));
                            offset += count;
                            await Task.Run(() => outStreamDecrypted.Write(data, 0, count));
                        }
                        while (count > 0);

                        outStreamDecrypted.FlushFinalBlock();
                        outStreamDecrypted.Close();
                    }
                    outFs.Close();
                }

                inFs.Close();
            }
        }
    }
}
