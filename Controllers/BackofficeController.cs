using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace MongoDB_Code.Controllers
{
    public class BackofficeController : Controller
    {
        private readonly MongoDBServiceProvider? _mongoDBServiceProvider;
        private readonly ILogger<BackofficeController> _logger;
        private readonly CryptoService? _cryptoService;
        private readonly IConfiguration _configuration;

        public BackofficeController(MongoDBServiceProvider? mongoDBServiceProvider, ILogger<BackofficeController> logger, CryptoService? cryptoService, IConfiguration configuration)
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

            if (_mongoDBServiceProvider == null)
            {
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

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
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            jsonObj["MyDatabaseSettings"]["ConnectionString"] = newSettings.ConnectionString;
            jsonObj["MyDatabaseSettings"]["DatabaseName"] = newSettings.DatabaseName;
            jsonObj["MyDatabaseSettings"]["CollectionName"] = newSettings.CollectionName;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(filePath, output);

            _logger.LogInformation("Settings saved successfully in BackofficeController SaveSettings method.");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Entering Index method in BackofficeController.");

            var settings = _mongoDBServiceProvider?.GetCurrentSettings() ?? new MyDatabaseSettings();
            if (string.IsNullOrEmpty(settings.ConnectionString))
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