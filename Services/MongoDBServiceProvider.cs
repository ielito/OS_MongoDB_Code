using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB.Driver;


namespace MongoDB_Code.Services
{
    public class MongoDBServiceProvider
    {
        private MyDatabaseSettings _settings;
        private readonly ILogger<MongoDBService> _logger;
        private MongoDBService _mongoDBService;
        private readonly CryptoService _cryptoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;

        public MongoDBServiceProvider(IOptions<MyDatabaseSettings> settings, ILogger<MongoDBService> logger, CryptoService cryptoService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;

            if (string.IsNullOrEmpty(settings.Value.ConnectionString))
            {
                _logger.LogError("Connection string is empty. MongoDB client will not be initialized.");
                _mongoClient = null;
                _database = null;
            }
            else
            {
                try
                {
                    _mongoClient = new MongoClient(settings.Value.ConnectionString);
                    _database = _mongoClient.GetDatabase(settings.Value.DatabaseName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing MongoDB client.");
                    throw;
                }
            }

            _settings = settings.Value;
            _cryptoService = cryptoService;
            _mongoDBService = new MongoDBService(_settings, _logger);
            _httpContextAccessor = httpContextAccessor;
        }

        public void UpdateSettings(MyDatabaseSettings newSettings)
        {
            _settings = newSettings;
            _mongoDBService = new MongoDBService(_settings, _logger);
        }

        public MongoDBService CreateService()
        {
            return _mongoDBService;
        }

        public MyDatabaseSettings GetCurrentSettings()
        {
            _logger.LogInformation("Using connection string: {ConnectionString}", _settings.ConnectionString);

            var encryptionKey = _httpContextAccessor.HttpContext.Session.GetString("EncryptionKey");

            if (!string.IsNullOrEmpty(_settings.ConnectionString))
            {
                if (CryptoService.IsBase64String(_settings.ConnectionString))
                {
                    _logger.LogInformation("Decrypting connection string.");
                    var decryptedConnectionString = CryptoService.DecryptString(_settings.ConnectionString, encryptionKey);
                    _settings.ConnectionString = decryptedConnectionString;
                }
                else
                {
                    _logger.LogInformation("Connection string is not encrypted.");
                }
            }
            else
            {
                _logger.LogWarning("Connection string is empty.");
            }

            return _settings;
        }
    }
}