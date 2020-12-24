using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestingTransferProtocol.Config;
using TransferProtocolLibrary;
using TransferProtocolLibrary.Config;
using TransferProtocolLibrary.DataManagement;
using TransferProtocolLibrary.Logging;

namespace TestingTransferProtocol
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

            System.Console.WriteLine(projectPath);

            Configuration config = ConfigManager.GetOptions<Configuration>(projectPath, "config*");

            DataManager dataManager = new DataManager(config);

            ILogger logger = new LogManager(dataManager.databaseManager);
            logger.Setup();

            DirectoryWatcher directoryWatcher = new DirectoryWatcher(config.SourcePath, config.TargetPath);
            directoryWatcher.SetEncriptorKey(config.Key);

            dataManager.CreateXMLFilesAsync();
            directoryWatcher.Start();
        }
    }
}
