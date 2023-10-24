using OutSystems.ExternalLibraries.SDK;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using DnsClient.Protocol;

public class MongoDBOperations : IMongoDBOperations
{
    private readonly MongoDBServiceProvider? _mongoDBServiceProvider;
    private readonly ILogger<MongoDBOperations>? _logger;

    public MongoDBOperations()
    {
        var loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<MongoDBService>();
        _mongoDBServiceProvider = new MongoDBServiceProvider(logger);
    }

    public MongoDBOperations(ILogger<MongoDBOperations> logger, MongoDBServiceProvider mongoDBServiceProvider)
    {
        _logger = logger;
        _mongoDBServiceProvider = mongoDBServiceProvider;
    }

    public void InitializeMongoDB(string connectionString, string databaseName, string collectionName)
    {
        var settings = new MyDatabaseSettings
        {
            ConnectionString = connectionString,
            DatabaseName = databaseName,
            CollectionName = collectionName
        };
        if (_logger == null)
        {
            throw new ArgumentNullException(nameof(_logger), "Logger is not initialized.");
        }
        if (_mongoDBServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(_mongoDBServiceProvider), "MongoDB is not initialized");
            }
        _mongoDBServiceProvider.Initialize(settings);
        _logger.LogInformation("MongoDB initialized successfully.");
    }

        public void SaveSettings(string connectionString, string databaseName, string collectionName)
        {
            var newSettings = new MyDatabaseSettings
            {
                ConnectionString = connectionString,
                DatabaseName = databaseName,
                CollectionName = collectionName
            };
            _mongoDBServiceProvider?.UpdateSettings(newSettings);
        }

        public MongoDBSettings GetSettings()
        {
            if (_mongoDBServiceProvider == null)
            {
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

            var settings = _mongoDBServiceProvider.GetCurrentSettings();
            if (settings == null)
            {
                throw new InvalidOperationException("Database settings have not been initialized.");
            }

            return new MongoDBSettings
            {
                ConnectionString = settings.ConnectionString,
                DatabaseName = settings.DatabaseName,
                CollectionName = settings.CollectionName
            };
        }

        public List<MongoDBRecord> GetAllRecordsFromCollection(string connectionString, string databaseName, string collectionName)
        {
            try
            {
                LogInformation("Starting GetAllRecordsFromCollection.");

                // Inicializar o provedor de serviços MongoDB com as configurações fornecidas
                var settings = new MyDatabaseSettings
                {
                    ConnectionString = connectionString,
                    DatabaseName = databaseName,
                    CollectionName = collectionName
                };

                if (_mongoDBServiceProvider == null)
                {
                    throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
                }

                _mongoDBServiceProvider.Initialize(settings);

                var records = new List<MongoDBRecord>();

                var collection = _mongoDBServiceProvider.GetCollection<BsonDocument>(collectionName);

                if (collection == null)
                {
                    throw new InvalidOperationException($"Unable to get collection: {collectionName}");
                }

                var documents = collection.Find(_ => true).ToList();

                LogInformation($"Found {documents.Count} documents in collection: {collectionName}.");

                foreach (var document in documents)
                {
                    var json = document.ToJson();
                    records.Add(new MongoDBRecord { Data = json });
                }

                LogInformation("Finished GetAllRecordsFromCollection.");
                return records;
            }
            catch (Exception ex)
            {
                LogError($"Error in GetAllRecordsFromCollection: {ex.Message}. StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private void LogInformation(string message)
        {
            _logger?.LogInformation(message);
        }

        private void LogError(string message)
        {
            _logger?.LogError(message);
        }
}
 
