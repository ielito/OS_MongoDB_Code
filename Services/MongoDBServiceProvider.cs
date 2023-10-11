using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;



namespace MongoDB_Code.Services
{
    public class MongoDBServiceProvider
    {
        private MyDatabaseSettings _settings;
        private readonly ILogger<MongoDBService> _logger;
        private MongoDBService _mongoDBService;

        public MongoDBServiceProvider(IOptions<MyDatabaseSettings> settings, ILogger<MongoDBService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _mongoDBService = new MongoDBService(_settings, _logger);
        }

        public MongoDBService CreateService()
        {
            return _mongoDBService;
        }

        public void UpdateSettings(MyDatabaseSettings newSettings)
        {
            _settings = newSettings;
            _mongoDBService = new MongoDBService(_settings, _logger); // Recria o serviço com as novas configurações
        }

        public MyDatabaseSettings GetCurrentSettings()
        {
            return _settings;
        }
    }
}