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
            var mongoService = _mongoDBServiceProvider.CreateService();
            var data = await mongoService.RetrieveDataAsync();
            return View(data);
        }
    }
}
