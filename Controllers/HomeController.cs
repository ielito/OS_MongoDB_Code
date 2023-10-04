using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Services;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB_Code.Models;
using System.Diagnostics;

namespace MongoDB_Code.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public HomeController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<IActionResult> Index()
        {
            // Buscar os dados do MongoDB
            List<BsonDocument> documents = await _mongoDBService.RetrieveDataAsync();

            // Passar os dados para a View
            return View(documents);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
