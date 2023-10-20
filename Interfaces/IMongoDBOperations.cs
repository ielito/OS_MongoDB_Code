using Amazon.Runtime.Documents;
using OutSystems.ExternalLibraries.SDK;

[OSInterface]
public interface IMongoDBOperations
{
    void InitializeMongoDB(string connectionString, string databaseName, string collectionName);
    void SaveSettings(string connectionString, string databaseName, string collectionName);
    MongoDBSettings GetSettings();
    List<MongoDBRecord> GetAllRecordsFromCollection(string connectionString, string databaseName, string collectionName);
}

[OSStructure]
public struct MongoDBSettings
{
    public string ConnectionString;
    public string DatabaseName;
    public string CollectionName;
}

[OSStructure]
public struct MongoDBRecord
{
    public string Data;
}