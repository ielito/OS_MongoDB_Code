﻿using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using MongoDB.Driver;
using MongoDB.Bson;

public class MongoDBServiceProvider
{
    private MyDatabaseSettings? _settings;
    private ILogger<MongoDBService> _logger;
    private MongoDBService _mongoDBService;
    private MongoClient? _mongoClient;
    private IMongoDatabase? _database;
    private IMongoCollection<BsonDocument>? _collection;

    public void Initialize()
    {
        if (string.IsNullOrEmpty(_settings?.ConnectionString))
        {
            _logger.LogError("Connection string is empty. MongoDB client is not initialized.");
            return;
        }
        _mongoClient = new MongoClient(_settings.ConnectionString);
        var database = _mongoClient.GetDatabase(_settings.DatabaseName);
        _collection = database.GetCollection<BsonDocument>(_settings.CollectionName);
    }

    public MongoDBServiceProvider(IOptions<MyDatabaseSettings> settings, ILogger<MongoDBService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (settings?.Value == null || string.IsNullOrEmpty(settings.Value.ConnectionString))
        {
            _logger.LogError("Connection string is empty. MongoDB client will not be initialized.");
            _mongoClient = null;
            _database = null;
            _settings = new MyDatabaseSettings(); // Valor padrão
        }
        else
        {
            _mongoClient = new MongoClient(settings.Value.ConnectionString);
            _database = _mongoClient.GetDatabase(settings.Value.DatabaseName);
            _settings = settings.Value;
        }

        _mongoDBService = new MongoDBService(_settings, _logger);
    }

    public void UpdateSettings(MyDatabaseSettings newSettings)
    {
        _settings = newSettings.DeepCopy();
        _mongoDBService = new MongoDBService(_settings, _logger);
    }

    public MongoDBService CreateService()
    {
        return _mongoDBService ?? throw new InvalidOperationException("MongoDBService is not initialized.");
    }

    public MyDatabaseSettings GetCurrentSettings()
    {
        _logger.LogInformation("Using connection string: {ConnectionString}", _settings.ConnectionString);
        return _settings.DeepCopy();
    }
}