using Amazon.Runtime.Documents;
using OutSystems.ExternalLibraries.SDK;

[OSInterface]
public interface IMongoDBOperations
{
    void InitializeMongoDB();
    void SaveSettings(string connectionString, string databaseName, string collectionName);
    MongoDBSettings GetSettings();
}

[OSStructure]
public struct MongoDBSettings
{
    public string ConnectionString;
    public string DatabaseName;
    public string CollectionName;
}