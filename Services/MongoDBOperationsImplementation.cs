using MongoDB_Code.Models;

public class MongoDBOperationsImplementation : IMongoDBOperations
{
    private readonly MongoDBServiceProvider _mongoDBServiceProvider;

    public MongoDBOperationsImplementation(MongoDBServiceProvider mongoDBServiceProvider)
    {
        _mongoDBServiceProvider = mongoDBServiceProvider ?? throw new ArgumentNullException(nameof(mongoDBServiceProvider));
    }

    public void SaveSettings(string connectionString, string databaseName, string collectionName)
    {
        var settings = new MyDatabaseSettings
        {
            ConnectionString = connectionString,
            DatabaseName = databaseName,
            CollectionName = collectionName
        };
        _mongoDBServiceProvider.UpdateSettings(settings);
    }

    public MongoDBSettings GetSettings()
    {
        var currentSettings = _mongoDBServiceProvider.GetCurrentSettings();
        return new MongoDBSettings
        {
            ConnectionString = currentSettings.ConnectionString,
            DatabaseName = currentSettings.DatabaseName,
            CollectionName = currentSettings.CollectionName
        };
    }

    public void InitializeMongoDB()
    {
        _mongoDBServiceProvider.Initialize();
    }
}