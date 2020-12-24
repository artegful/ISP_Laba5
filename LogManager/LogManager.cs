using System;
using System.Collections.Generic;
using System.Linq;
using TransferProtocolLibrary.DataManagement;
using TransferProtocolLibrary.DataManagement.Database;

namespace TransferProtocolLibrary.Logging
{
    public class LogManager : ILogger
    {
        DatabaseManager databaseManager;

        private const string tableName = "Errors";

        private string[] columnsTypes = { "Time varchar(50)", "Message varchar(100)" };
        private string[] columnsNames = { "Time", "Message" };

        public LogManager(DatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;

            TryCreateErrorTable();
        }

        public async void TryCreateErrorTable()
        {
            bool hasErrorTable = databaseManager.HasTable(tableName);

            if (!hasErrorTable)
            {
                databaseManager.CreateTableAsync(tableName, columnsTypes);
            }
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            LogWithTime(message);
            LogDatabase(message);
            Console.ResetColor();
        }

        public void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            LogWithTime(message);
            LogDatabase(message);
            Console.ResetColor();
        }

        public void LogCreation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            LogWithTime(message);
            LogDatabase(message);
            Console.ResetColor();
        }

        public void LogMessage(string message)
        {
            LogWithTime(message);
        }

        public void LogWithTime(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("G") + message);
        }

        private void LogDatabase(string message)
        {
            databaseManager.InsertValueAsync(tableName, columnsNames, new string[] { DateTime.Now.ToString("y/M/d h.m.ss"), message });
        }

        public void Setup()
        {
            DirectoryWatcher.OnSuccess += LogSuccess;
            DirectoryWatcher.OnFileTransfer += LogMessage;
            DataManager.OnFileCreation += LogCreation;
        }
    }
}
