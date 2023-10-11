using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.IO;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using MongoDB.Driver;
using Microsoft.Extensions.Logging; // Adicione esta linha para usar logging

namespace MongoDB_Code.Controllers
{
    public class BackofficeController : Controller
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;
        private readonly ILogger<BackofficeController> _logger;
        private readonly CryptoService? _cryptoService;
        private readonly string? _encryptionKey;
        private readonly IConfiguration _configuration;

        public BackofficeController(MongoDBServiceProvider mongoDBServiceProvider, ILogger<BackofficeController> logger, CryptoService cryptoService, IConfiguration configuration)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
            _logger = logger;
            _cryptoService = cryptoService;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Save(BackofficeModel model)
        {
            _logger.LogInformation("Entering Save method in BackofficeController."); // Log de informação

            if (ModelState.IsValid)
            {
                string encryptionKey = model.EncryptionKey;

                if (string.IsNullOrEmpty(model.EncryptionKey))
                {
                    _logger.LogWarning("Encryption key is empty in BackofficeController Save method."); // Log de aviso
                    ModelState.AddModelError(string.Empty, "Encryption key is required.");
                    return View("Index", model);
                }

                HttpContext.Session.SetString("EncryptionKey", model.EncryptionKey);

                var encryptedConnectionString = CryptoService.EncryptString(model.ConnectionString, model.EncryptionKey);

                var newSettings = new MyDatabaseSettings
                {
                    ConnectionString = encryptedConnectionString,
                    DatabaseName = model.DatabaseName,
                    CollectionName = model.CollectionName
                };

                SaveSettings(newSettings);

                _mongoDBServiceProvider.UpdateSettings(newSettings);

                TempData["SuccessMessage"] = "Configurações salvas com sucesso!";
                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Model state is invalid in BackofficeController Save method."); // Log de aviso
            return View("Index", model);
        }

        private void SaveSettings(MyDatabaseSettings newSettings)
        {
            _logger.LogInformation("Entering SaveSettings method in BackofficeController."); // Log de informação

            string filePath = "appsettings.json";
            var json = System.IO.File.ReadAllText(filePath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            jsonObj["MyDatabaseSettings"]["ConnectionString"] = newSettings.ConnectionString;
            jsonObj["MyDatabaseSettings"]["DatabaseName"] = newSettings.DatabaseName;
            jsonObj["MyDatabaseSettings"]["CollectionName"] = newSettings.CollectionName;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(filePath, output);

            _logger.LogInformation("Settings saved successfully in BackofficeController SaveSettings method."); // Log de informação
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Entering Index method in BackofficeController."); // Log de informação

            var settings = _mongoDBServiceProvider.GetCurrentSettings();

            if (string.IsNullOrEmpty(settings.ConnectionString))
            {
                _logger.LogWarning("Connection string is empty in BackofficeController Index method."); // Log de aviso
                ViewBag.ErrorMessage = "A string de conexão está vazia. Por favor, configure-a antes de prosseguir.";
                return View("Error");
            }

            string encryptionKey = HttpContext.Session.GetString("EncryptionKey");
            var decryptedConnectionString = CryptoService.DecryptString(settings.ConnectionString, encryptionKey);

            var model = new BackofficeModel
            {
                ConnectionString = decryptedConnectionString,
                DatabaseName = settings.DatabaseName,
                CollectionName = settings.CollectionName
            };

            return View(model);
        }
    }
}
