using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Services;
using System.Threading.Tasks;

namespace MongoDB_Code.Controllers
{
    public class DataController : Controller
    {
        private readonly MongoDBServiceProvider _mongoDBServiceProvider;

        public DataController(MongoDBServiceProvider mongoDBServiceProvider)
        {
            _mongoDBServiceProvider = mongoDBServiceProvider;
        }

        public async Task<IActionResult> Index()
        {
            if (_mongoDBServiceProvider == null)
            {
                throw new InvalidOperationException("MongoDBServiceProvider is not initialized.");
            }

            // Supondo que RetrieveDataAsync seja um método do MongoDBServiceProvider
            var data = await _mongoDBServiceProvider.RetrieveDataAsync();
            return View(data);
        }
    }
}
