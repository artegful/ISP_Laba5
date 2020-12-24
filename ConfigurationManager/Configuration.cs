using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferProtocolLibrary.DataManagement;

namespace TestingTransferProtocol.Config
{
    public class Configuration : IConfigDatabase
    {
        public Configuration(string SourcePath, string TargetPath, string Key, string TargetDataPath, string ConnectionString, string DatabaseName)
        {
            this.SourcePath = SourcePath;
            this.TargetPath = TargetPath;
            this.Key = Key;
            this.ConnectionString = ConnectionString;
            this.TargetDataPath = TargetDataPath;
            this.DatabaseName = DatabaseName;
        }

        public string SourcePath;

        public string TargetPath;

        public string Key;

        public string ConnectionString;

        public string DatabaseName;

        public string TargetDataPath;

        public string GetTargetForDataManager() => TargetDataPath;

        public string GetConnectionString() => ConnectionString;

        public string GetDatabaseName() => DatabaseName;
    }
}
