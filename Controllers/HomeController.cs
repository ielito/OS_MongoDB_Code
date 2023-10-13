using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Services;
using MongoDB_Code.Models;
using System.Diagnostics;

namespace MongoDB_Code.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MongoDBServiceProvider mongoDBServiceProvider, ILogger<HomeController> logger)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering Index method in HomeController.");

            var mongoDBService = _mongoDBServiceProvider.CreateService();
            var model = new IndexModel(mongoDBService);

            try
            {
                await model.RetrieveDataAsync(10);
                _logger.LogInformation("Data retrieved successfully in HomeController Index method.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data in HomeController Index method: {ex.Message}");
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Accessing Privacy view in HomeController.");
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning("Entering Error view in HomeController.");

            var settings = _mongoDBServiceProvider.GetCurrentSettings();
            if (string.IsNullOrEmpty(settings.ConnectionString))
            {
                _logger.LogWarning("Connection string is empty. Redirecting to Backoffice Index view from HomeController Error method.");
                return RedirectToAction("Index", "Backoffice");
            }

            _logger.LogInformation("Displaying Error view in HomeController.");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
