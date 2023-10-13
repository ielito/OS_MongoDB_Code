﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_Code.Models;

namespace MongoDB_Code.Services
{
    public class MongoDBService
        {
            private MyDatabaseSettings _settings;
            private readonly ILogger<MongoDBService> _logger;
            private IMongoCollection<BsonDocument> _collection;
            private readonly MongoClient _mongoClient;
            private readonly IMongoDatabase _database;

            public MongoDBService(MyDatabaseSettings settings, ILogger<MongoDBService> logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _settings = settings ?? throw new ArgumentNullException(nameof(settings));

                if (string.IsNullOrEmpty(settings.ConnectionString))
                {
                    _logger.LogError("Connection string is empty. MongoDB client is not initialized.");
                    return;
                }

                _mongoClient = new MongoClient(settings.ConnectionString);


                _settings = settings ?? throw new ArgumentNullException(nameof(settings));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                var client = new MongoClient(_settings.ConnectionString);
                _mongoClient = new MongoClient(settings.ConnectionString);
                var database = _mongoClient.GetDatabase(_settings.DatabaseName);
                _collection = database.GetCollection<BsonDocument>(_settings.CollectionName);
                _mongoClient = new MongoClient(settings.ConnectionString);
        
        }

            public void UpdateConfiguration(MyDatabaseSettings settings)
            {
                 _settings = settings;

                var client = new MongoClient(_settings.ConnectionString);
                var database = client.GetDatabase(_settings.DatabaseName);
                _collection = database.GetCollection<BsonDocument>(_settings.CollectionName);
            }

            public MyDatabaseSettings GetCurrentSettings()
            {
                return _settings;
            }

        public async Task<List<BsonDocument>> RetrieveDataAsync(int limit = 1)
        {
            try
            {
                if (_collection == null)
                {
                    _logger.LogError("MongoDB collection is not configured.");
                    return new List<BsonDocument>();
                }

                return await _collection.Find(new BsonDocument()).Limit(limit).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving data from MongoDB: {ex.Message}");
                return new List<BsonDocument>();
            }
        }
    }
}
