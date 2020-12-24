namespace TransferProtocolLibrary.DataManagement
{
    public interface IConfigDatabase
    {
        string GetConnectionString();

        string GetTargetForDataManager();

        string GetDatabaseName();
    }
}