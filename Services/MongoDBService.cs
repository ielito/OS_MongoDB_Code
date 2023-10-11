using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_Code.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MongoDB_Code.Services
{
    public class MongoDBService
    {
        private MyDatabaseSettings _settings;
        private readonly ILogger<MongoDBService> _logger;
        private IMongoCollection<BsonDocument> _collection;

        public MongoDBService(MyDatabaseSettings settings, ILogger<MongoDBService> logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _collection = database.GetCollection<BsonDocument>(_settings.CollectionName);
        }

        public void UpdateConfiguration(MyDatabaseSettings settings)
        {
            // Atualizando as configurações internas
             _settings = settings;

            // Reconfigurando o cliente e a coleção do MongoDB
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
                    throw new InvalidOperationException("MongoDB collection is not configured.");
                }

                return await _collection.Find(new BsonDocument()).Limit(limit).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving data from MongoDB: {ex.Message}");
                return new List<BsonDocument>();
            }
        }

        // Outros métodos para interagir com o MongoDB...
    }
}
