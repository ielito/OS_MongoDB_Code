using OutSystems.ExternalLibraries.SDK;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Amazon.Runtime.Internal.Util;
using DnsClient.Internal;

namespace MongoDB_Code.Operations
{
    public class MongoDBOperations : IMongoDBOperations
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;
        private readonly ILogger<MongoDBOperations> _logger;

        public MongoDBOperations(ILogger<MongoDBOperations> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ILogger<MongoDBService> mongoServiceLogger = new Logger<MongoDBService>(new LoggerFactory());
            _mongoDBServiceProvider = new MongoDBServiceProvider(mongoServiceLogger);
        }

        public void InitializeMongoDB(string connectionString, string databaseName, string collectionName)
        {
            var settings = new MyDatabaseSettings
            {
                ConnectionString = connectionString,
                DatabaseName = databaseName,
                CollectionName = collectionName
            };
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
            _mongoDBServiceProvider.UpdateSettings(newSettings);
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
            // Criar uma nova instância do MongoDBServiceProvider com as configurações fornecidas
            var settings = new MyDatabaseSettings
            {
                ConnectionString = connectionString,
                DatabaseName = databaseName,
                CollectionName = collectionName
            };
            _mongoDBServiceProvider.Initialize(settings);

            var records = new List<MongoDBRecord>();

            // Obtenha a coleção do MongoDB
            var collection = _mongoDBServiceProvider.GetCollection<BsonDocument>(collectionName);

            // Consulte todos os registros da coleção
            var documents = collection.Find(_ => true).ToList();

            // Converta cada documento BSON em uma string JSON e adicione à lista
            foreach (var document in documents)
            {
                var json = document.ToJson();
                records.Add(new MongoDBRecord { Data = json });
            }

            return records;
        }
    }
}
