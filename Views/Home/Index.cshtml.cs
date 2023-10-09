using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB_Code.Services;

namespace MongoDB_Code.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MongoDBService _mongoDBService;

        public IndexModel(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public List<BsonDocument> Documents { get; private set; } = new List<BsonDocument>();

        public async Task OnGetAsync(int? limit)
        {
            ViewData["Title"] = "Home Page";
            Documents = await _mongoDBService.RetrieveDataAsync(limit ?? 1);
        }
    }
}
