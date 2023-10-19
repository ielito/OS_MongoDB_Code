using MongoDB_Code.Models;
using MongoDB_Code.Services;

namespace MongoDB_Code.Operations
{
    public class MongoDBOperations : IMongoDBOperations
    {
        private readonly MongoDBServiceProvider? _mongoDBServiceProvider;

        public MongoDBOperations() { }

        public MongoDBOperations(MongoDBServiceProvider mongoDBServiceProvider)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
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
            var settings = _mongoDBServiceProvider.GetCurrentSettings();
            return new MongoDBSettings
            {
                ConnectionString = settings.ConnectionString,
                DatabaseName = settings.DatabaseName,
                CollectionName = settings.CollectionName
            };
        }
    }
}
