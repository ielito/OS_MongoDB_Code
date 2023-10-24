using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_Code.Models;
using MongoDB_Code.Services;

namespace MongoDB_Code.Services
{
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
        try
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException(nameof(newSettings));
            }

            _settings = newSettings;

            // Reconfigure the MongoDB client and database with the new settings
            _mongoClient = new MongoClient(_settings.ConnectionString);
            _database = _mongoClient.GetDatabase(_settings.DatabaseName);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError($"Error updating MongoDB settings: {ex.Message}");
            throw; // Re-throw the exception to ensure it's handled by the caller
        }
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

        public bool IsInitialized()
        {
            return _mongoClient != null; // Supondo que _mongoClient é a variável que você usa para manter a conexão com o MongoDB.
        }

    }
}