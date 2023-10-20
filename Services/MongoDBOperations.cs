using OutSystems.ExternalLibraries.SDK;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amazon.Runtime.Internal.Util;

namespace MongoDB_Code.Operations
{
    public class MongoDBOperations : IMongoDBOperations
    {
        private static readonly MongoDBServiceProvider _mongoDBServiceProviderSingleton;
        private readonly ILogger<MongoDBOperations> _logger;

        static MongoDBOperations()
        {
            var loggerFactory = new LoggerFactory();
            var defaultSettings = new MyDatabaseSettings
            {
                ConnectionString = "",
                DatabaseName = "",
                CollectionName = ""
            };
            var options = Options.Create(defaultSettings);
            _mongoDBServiceProviderSingleton = new MongoDBServiceProvider(options, new Logger<MongoDBService>(loggerFactory));
        }

        public MongoDBOperations()
        {
            _logger = new Logger<MongoDBOperations>(new LoggerFactory());
        }

        public void InitializeMongoDB()
        {
            if (_mongoDBServiceProviderSingleton == null)
            {
                _logger.LogError("Failed to initialize MongoDB: MongoDBServiceProvider is not initialized.");
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

            _logger.LogInformation("Initializing MongoDB...");
            _mongoDBServiceProviderSingleton.Initialize();
            _logger.LogInformation("MongoDB initialized successfully.");
        }

        public void SaveSettings(string connectionString, string databaseName, string collectionName)
        {
            if (_mongoDBServiceProviderSingleton == null)
            {
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

            var newSettings = new MyDatabaseSettings
            {
                ConnectionString = connectionString,
                DatabaseName = databaseName,
                CollectionName = collectionName
            };

            _mongoDBServiceProviderSingleton.UpdateSettings(newSettings);
        }

        public MongoDBSettings GetSettings()
        {
            if (_mongoDBServiceProviderSingleton == null)
            {
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

            var settings = _mongoDBServiceProviderSingleton.GetCurrentSettings();
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
