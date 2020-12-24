using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferProtocolLibrary.DataManagement.Database;
using TransferProtocolLibrary.DataManagement.Parsers;

namespace TransferProtocolLibrary.DataManagement
{
    public class DataManager
    {
        public static Action<string> OnFileCreation;

        private IXMLGenerator stringGenerator;
        public DatabaseManager databaseManager;

        private string targetPath;

        public DataManager(IConfigDatabase config)
        {
            databaseManager = new DatabaseManager(config.GetConnectionString(), config.GetDatabaseName());

            stringGenerator = new XMLGenerator();

            targetPath = config.GetTargetForDataManager();
        }

        public async void CreateXMLFilesAsync()
        {
            List<string> tableNames = databaseManager.GetAllTablesNames();
            List<List<List<KeyValuePair<string, object>>>> tableValues = new List<List<List<KeyValuePair<string, object>>>>();

            foreach (var tableName in tableNames)
            {
                List<List<KeyValuePair<string, object>>> data = await databaseManager.GetTableValuesAsync(tableName);
                tableValues.Add(data);
            }

            int i = 0;

            foreach (var tableName in tableNames)
            {
                string fileText = stringGenerator.GetXMLTableString(tableName,
                    tableValues[i++]);

                WriteXMLFileAsync(tableName, fileText);

                OnFileCreation?.Invoke($"Successfully created {tableName}.xml at {targetPath}");
            }
        }

        private async void WriteXMLFileAsync(string tableName, string fileText)
        {
            tableName = DeleteScemaFromName(tableName);

            string targetFile = Path.Combine(targetPath, tableName);
            targetFile = Path.ChangeExtension(targetFile, ".xml");

            using (StreamWriter sw = File.CreateText(targetFile))
            {
                await Task.Run(() => sw.WriteLine(fileText));
            }
        }

        private string DeleteScemaFromName(string name)
        {
            int dotIndex = name.IndexOf('.');
            name = name.Substring(dotIndex+1);
            return name;
        }

    }
}
