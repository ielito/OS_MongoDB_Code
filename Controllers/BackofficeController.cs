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

        public BackofficeController(MongoDBServiceProvider mongoDBServiceProvider)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
        }

        [HttpPost]
        public IActionResult Save(BackofficeModel model)
        {
            if (ModelState.IsValid)
            {
                var newSettings = new MyDatabaseSettings
                {
                    ConnectionString = model.ConnectionString,
                    DatabaseName = model.DatabaseName,
                    CollectionName = model.CollectionName
                };

                _mongoDBServiceProvider.UpdateSettings(newSettings);

                // Salvar as configurações no arquivo appsettings.json
                SaveSettings(newSettings);

                // Atualizar as configurações em memória e o serviço MongoDB
                _mongoDBServiceProvider.UpdateSettings(newSettings);

                TempData["SuccessMessage"] = "Configurações salvas com sucesso!";
                return RedirectToAction("Index", "Home");
            }

            return View("Index", model);
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
