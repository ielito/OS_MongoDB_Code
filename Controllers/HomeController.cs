using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Services;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB_Code.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging; // Adicione esta linha para usar logging

namespace MongoDB_Code.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;
        private readonly ILogger<HomeController> _logger; // Logger

        public HomeController(MongoDBServiceProvider mongoDBServiceProvider, ILogger<HomeController> logger) // Injete o logger
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
            _logger = logger; // Inicialize o logger
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering Index method in HomeController."); // Log de informação

            var mongoDBService = _mongoDBServiceProvider.CreateService();
            var model = new IndexModel(mongoDBService);

            try
            {
                await model.RetrieveDataAsync(10);
                _logger.LogInformation("Data retrieved successfully in HomeController Index method."); // Log de sucesso
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data in HomeController Index method: {ex.Message}"); // Log de erro
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Accessing Privacy view in HomeController."); // Log de informação
            return View();
        }

        public IActionResult Home()
        {
            // Lógica para a nova página Home, se necessário.
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning("Entering Error view in HomeController."); // Log de aviso

            var settings = _mongoDBServiceProvider.GetCurrentSettings();
            if (string.IsNullOrEmpty(settings.ConnectionString))
            {
                _logger.LogWarning("Connection string is empty. Redirecting to Backoffice Index view from HomeController Error method."); // Log de aviso
                return RedirectToAction("Index", "Backoffice");
            }

            _logger.LogInformation("Displaying Error view in HomeController."); // Log de informação
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
