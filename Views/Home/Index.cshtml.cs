using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB_Code.Services;
using MongoDB.Bson;

namespace MongoDB_Code.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHostEnvironment _env;
        private readonly MongoDBService _mongoDBService;

        public IndexModel(MongoDBService mongoDBService, IHostEnvironment env)
        {
            _mongoDBService = mongoDBService;
            _env = env;

        }

        public List<BsonDocument> Documents { get; private set; } = new List<BsonDocument>();

        public async Task OnGetAsync(int? limit)
        {
            ViewData["Title"] = "Home Page";
            Documents = await _mongoDBService.RetrieveDataAsync(limit ?? 1);
        }
    }
}