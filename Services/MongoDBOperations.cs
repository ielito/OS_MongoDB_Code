using OutSystems.ExternalLibraries.SDK;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using Microsoft.Extensions.Logging;
using Amazon.Runtime.Internal.Util;

namespace MongoDB_Code.Operations
{
    public class MongoDBOperations : IMongoDBOperations
    {
        private MongoDBServiceProvider? _mongoDBServiceProvider;
        private readonly ILogger<MongoDBOperations> _logger;

        // Adicionado um construtor público sem parâmetros
        public MongoDBOperations()
        {
            // Este construtor não faz nada, mas é necessário para a OutSystems
        }

        public MongoDBOperations(MongoDBServiceProvider mongoDBServiceProvider, ILogger<MongoDBOperations> logger)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Método setter para _mongoDBServiceProvider
        public void SetMongoDBServiceProvider(MongoDBServiceProvider mongoDBServiceProvider)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
        }

        public void InitializeMongoDB()
        {
            if (_mongoDBServiceProvider == null)
            {
                _logger?.LogError("Failed to initialize MongoDB: MongoDBServiceProvider is not initialized."); // Adicionado verificação nula para _logger
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

            _logger.LogInformation("Initializing MongoDB...");
            _mongoDBServiceProvider.Initialize();
            _logger.LogInformation("MongoDB initialized successfully.");
        }

        public void SaveSettings(string connectionString, string databaseName, string collectionName)
        {
            if (_mongoDBServiceProvider == null)
            {
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

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
    }
}