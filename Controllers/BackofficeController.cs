using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json;


namespace MongoDB_Code.Controllers
{
    public class BackofficeController : Controller
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;
        private readonly ILogger<BackofficeController> _logger;
        private readonly CryptoService _cryptoService;
        private readonly IConfiguration _configuration;

        public BackofficeController(MongoDBServiceProvider mongoDBServiceProvider, ILogger<BackofficeController> logger, CryptoService cryptoService, IConfiguration configuration)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider ?? throw new ArgumentNullException(nameof(mongoDBServiceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost]
        public IActionResult Save(BackofficeModel model)
        {
            _logger.LogInformation("Entering Save method in BackofficeController.");

            if (ModelState.IsValid)
            {
                var newSettings = new MyDatabaseSettings
                {
                    ConnectionString = model.ConnectionString,
                    DatabaseName = model.DatabaseName,
                    CollectionName = model.CollectionName
                };

                SaveSettings(newSettings);
                _mongoDBServiceProvider.UpdateSettings(newSettings);

                TempData["SuccessMessage"] = "Configurações salvas com sucesso!";
                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Model state is invalid in BackofficeController Save method.");
            return View("Index", model);
        }

        private void SaveSettings(MyDatabaseSettings newSettings)
        {
            _logger.LogInformation("Entering SaveSettings method in BackofficeController.");

            string filePath = "appsettings.json";
            var json = System.IO.File.ReadAllText(filePath);

            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (jsonObj == null)
            {
                throw new InvalidOperationException("Failed to deserialize the configuration.");
            }

            if (!jsonObj.ContainsKey("MyDatabaseSettings") || jsonObj["MyDatabaseSettings"] == null)
            {
                throw new InvalidOperationException("MyDatabaseSettings not found in the configuration.");
            }

            var databaseSettings = jsonObj["MyDatabaseSettings"] as Dictionary<string, object>;
            if (databaseSettings == null)
            {
                throw new InvalidOperationException("MyDatabaseSettings is not in the expected format.");
            }

            databaseSettings["ConnectionString"] = newSettings.ConnectionString;
            databaseSettings["DatabaseName"] = newSettings.DatabaseName;
            databaseSettings["CollectionName"] = newSettings.CollectionName;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, output);

            _logger.LogInformation("Settings saved successfully in BackofficeController SaveSettings method.");
        }


        public IActionResult Index()
        {
            _logger.LogInformation("Entering Index method in BackofficeController.");

            var settings = _mongoDBServiceProvider.GetCurrentSettings();
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                _logger.LogWarning("Connection string is empty in BackofficeController Index method.");
                ViewBag.ErrorMessage = "A string de conexão está vazia. Por favor, configure-a antes de prosseguir.";
                return View("Index");
            }

            var model = new BackofficeModel
            {
                ConnectionString = settings.ConnectionString,
                DatabaseName = settings.DatabaseName,
                CollectionName = settings.CollectionName
            };

            return View(model);
        }
    }
}
