using OutSystems.ExternalLibraries.SDK;
using MongoDB_Code.Models;
using MongoDB_Code.Services;

namespace MongoDB_Code.Operations
{
    public class MongoDBOperations : IMongoDBOperations
    {
        private readonly MongoDBServiceProvider? _mongoDBServiceProvider;

        public MongoDBOperations() { }

        public void InitializeMongoDB()
        {
            _mongoDBServiceProvider.Initialize();
        }

        public MongoDBOperations(MongoDBServiceProvider mongoDBServiceProvider)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
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