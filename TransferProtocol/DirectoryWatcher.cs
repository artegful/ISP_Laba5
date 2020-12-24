using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TransferProtocolLibrary.Compression;

namespace TransferProtocolLibrary
{
    public class DirectoryWatcher
    {
        private FileSystemWatcher fileWatcher;

        private bool enabled = false;

        private string sourcePath;
        private string targetPath;

        public static event Action<string> OnFileTransfer;
        public static event Action<string> OnError;
        public static event Action<string> OnSuccess;

        public DirectoryWatcher(string sourcePath, string targetPath)
        {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;

            SetupFileSystemWatcher();
        }

        private void SetupFileSystemWatcher()
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = sourcePath;

            fileWatcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName;

            fileWatcher.Changed += OnChanged;
            fileWatcher.Created += OnChanged;
            fileWatcher.Renamed += OnChanged;

            fileWatcher.IncludeSubdirectories = true;
        }

        public void SetEncriptorKey(string key)
        {
            Encriptor.SetKeys(key);
        }
        public void Start()
        {
            fileWatcher.EnableRaisingEvents = true;
            enabled = true;

            OnSuccess?.Invoke("FileWatcher Started Working");

            while (enabled)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            OnSuccess?.Invoke("FileWatcher Stopped Working.");

            fileWatcher.EnableRaisingEvents = false;
            enabled = false;
        }


        private async void OnChanged(object sender, FileSystemEventArgs args)
        {
            if (Path.GetExtension(args.FullPath) == "")
            {
                return;
            }

            await TransferFileAsync(args.FullPath);
        }

        private async Task TransferFileAsync(string path)
        {
            try
            {
                OnFileTransfer?.Invoke($"Transfering {path}...");

                string datePath = Path.Combine(targetPath, DateTime.Now.ToString("dd.MM.yyyy"));
                string targetFileName = Path.GetFileNameWithoutExtension(path) +
                    DateTime.Now.ToString("_dd_MM_yyy_H_mm_ss");
                targetFileName = Path.ChangeExtension(targetFileName, Path.GetExtension(path));

                Directory.CreateDirectory(datePath);
                string encriptedFilePath = Path.Combine(datePath, targetFileName);
                encriptedFilePath = Path.ChangeExtension(encriptedFilePath, ".enc");
                await Encriptor.EncryptFileAsync(path, encriptedFilePath);

                string compressedFilePath = Path.ChangeExtension(encriptedFilePath, ".gz");
                await Compressor.CompressAsync(encriptedFilePath, compressedFilePath);

                string decompressedFilePath = Path.Combine(datePath, "Decompressed");
                DirectoryInfo directoryDecompressedInfo = Directory.CreateDirectory(decompressedFilePath);
                decompressedFilePath = Path.Combine(directoryDecompressedInfo.FullName, targetFileName);
                decompressedFilePath = Path.ChangeExtension(decompressedFilePath, ".enc");
                await Compressor.DecompressAsync(compressedFilePath, decompressedFilePath);

                string decriptedFilePath = Path.Combine(datePath, "Decripted");
                DirectoryInfo directoryDecriptedInfo = Directory.CreateDirectory(decriptedFilePath);
                decriptedFilePath = Path.Combine(directoryDecriptedInfo.FullName, targetFileName);

                await Encriptor.DecryptFileAsync(encriptedFilePath, decriptedFilePath);

                OnSuccess?.Invoke("File Transfered, encrypted and compressed successfully");
            }
            catch
            {
                OnError?.Invoke("Error compressing file");
            }
        }
    }
}
