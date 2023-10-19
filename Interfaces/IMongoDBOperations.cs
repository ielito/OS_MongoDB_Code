using OutSystems.ExternalLibraries.SDK;

[OSInterface]
public interface IMongoDBOperations
{
    void SaveSettings(string connectionString, string databaseName, string collectionName);
    MongoDBSettings GetSettings();
    void InitializeMongoDB();
}

[OSStructure]
public struct MongoDBSettings
{
    public string ConnectionString;
    public string DatabaseName;
    public string CollectionName;
}