using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB.Driver;
using Newtonsoft.Json;


namespace MongoDB_Code.Services
{

    using Newtonsoft.Json;

    public static class Extensions
    {
        public static T DeepCopy<T>(this T obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }



    public class MongoDBServiceProvider
    {
        private MyDatabaseSettings? _settings;
        private readonly ILogger<MongoDBService> _logger;
        private MongoDBService? _mongoDBService;
        private readonly CryptoService _cryptoService;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly MongoClient? _mongoClient;
        private readonly IMongoDatabase? _database;

        public MongoDBServiceProvider(IOptions<MyDatabaseSettings> settings, ILogger<MongoDBService> logger, CryptoService cryptoService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

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
            _mongoDBService = new MongoDBService(_settings, _logger);
        }

        public void UpdateSettings(MyDatabaseSettings newSettings)
        {
            _settings = newSettings;
            _mongoDBService = new MongoDBService(_settings, _logger);
        }

        public MongoDBService? CreateService()
        {
            return _mongoDBService;
        }

        public MyDatabaseSettings? GetCurrentSettings()
        {
            _logger.LogInformation("Using connection string: {ConnectionString}", _settings.ConnectionString);

            var encryptionKey = _httpContextAccessor.HttpContext.Session.GetString("EncryptionKey");

            var clonedSettings = _settings.DeepCopy();

            if (!string.IsNullOrEmpty(clonedSettings.ConnectionString))
            {
                if (CryptoService.IsBase64String(clonedSettings.ConnectionString))
                {
                    _logger.LogInformation("Decrypting connection string.");
                    var decryptedConnectionString = CryptoService.DecryptString(clonedSettings.ConnectionString, encryptionKey);
                    clonedSettings.ConnectionString = decryptedConnectionString;
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

            return clonedSettings;
        }
    }
}
