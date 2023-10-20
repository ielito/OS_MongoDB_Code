using MongoDB_Code.Models;
using MongoDB_Code.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoDBServiceProvider
{
    private MyDatabaseSettings? _settings;
    private readonly ILogger<MongoDBService> _logger;
    private MongoClient? _mongoClient;
    private IMongoDatabase? _database;

    public MongoDBServiceProvider(ILogger<MongoDBService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Initialize(MyDatabaseSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _mongoClient = new MongoClient(_settings.ConnectionString);
        _database = _mongoClient.GetDatabase(_settings.DatabaseName);
    }

    public void UpdateSettings(MyDatabaseSettings newSettings)
    {
        _settings = newSettings.DeepCopy() ?? throw new ArgumentNullException(nameof(newSettings));
    }

    public MyDatabaseSettings? GetCurrentSettings()
    {
        if (_settings == null)
        {
            _logger.LogWarning("Settings are not initialized.");
            return null; // Retorne null ou lance uma exceção, dependendo do comportamento desejado.
        }

        _logger.LogInformation("Using connection string: {ConnectionString}", _settings.ConnectionString);
        return _settings.DeepCopy();
    }

    public async Task<List<BsonDocument>> RetrieveDataAsync()
    {
        if (_database == null || _settings == null)
        {
            throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
        }

        var collection = _database.GetCollection<BsonDocument>(_settings.CollectionName);
        return await collection.Find(new BsonDocument()).ToListAsync();
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName) where T : class
    {
        if (string.IsNullOrEmpty(collectionName))
        {
            throw new ArgumentException("Collection name cannot be null or empty.", nameof(collectionName));
        }

        if (_database == null)
        {
            throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
        }

        return _database.GetCollection<T>(collectionName);
    }
}
