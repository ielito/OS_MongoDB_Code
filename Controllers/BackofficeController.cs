using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace MongoDB_Code.Controllers
{
    public class BackofficeController : Controller
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;
        private readonly ILogger<BackofficeController> _logger;

        public BackofficeController(MongoDBServiceProvider mongoDBServiceProvider, ILogger<BackofficeController> logger)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
            _logger = logger; // Agora logger é passado como um argumento para o construtor
        }

        [HttpPost]
        public IActionResult Save(BackofficeModel model)
        {
            _logger.LogInformation("Save action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid.");
                return View("Index", model);
            }

            var newSettings = new MyDatabaseSettings
            {
                ConnectionString = model.ConnectionString,
                DatabaseName = model.DatabaseName,
                CollectionName = model.CollectionName
            };

            _mongoDBServiceProvider.UpdateSettings(newSettings);
            SaveSettings(newSettings);

            _logger.LogInformation("Settings saved. Setting TempData and redirecting...");
            TempData["SuccessMessage"] = "Configurações salvas com sucesso!";
            return RedirectToAction("Index", "Home");
        }


        private void SaveSettings(MyDatabaseSettings newSettings)
        {
            // Caminho para o arquivo appsettings.json
            string filePath = "appsettings.json";

            // Ler o arquivo JSON existente
            var json = System.IO.File.ReadAllText(filePath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            // Atualizar as configurações
            jsonObj["MyDatabaseSettings"]["ConnectionString"] = newSettings.ConnectionString;
            jsonObj["MyDatabaseSettings"]["DatabaseName"] = newSettings.DatabaseName;
            jsonObj["MyDatabaseSettings"]["CollectionName"] = newSettings.CollectionName;

            // Escrever o objeto JSON atualizado de volta ao arquivo
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(filePath, output);
        }

        public IActionResult Index()
        {
            var settings = _mongoDBServiceProvider.GetCurrentSettings();

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
