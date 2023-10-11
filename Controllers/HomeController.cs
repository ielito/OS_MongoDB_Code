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
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;

        public HomeController(MongoDBServiceProvider mongoDBServiceProvider)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
        }

        public async Task<IActionResult> Index()
        {
            var mongoDBService = _mongoDBServiceProvider.CreateService();
            var model = new IndexModel(mongoDBService);
            await model.RetrieveDataAsync(10);
            return View(model);
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
