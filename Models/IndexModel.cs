using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB_Code.Services;

namespace MongoDB_Code.Models
{
    public class IndexModel : PageModel
    {
        private readonly MongoDBService _mongoDBService;
        public IList<BsonDocument> Documents { get; private set; } = new List<BsonDocument>();

        public IndexModel(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task OnGetAsync()
        {
            Documents = await _mongoDBService.RetrieveDataAsync();
        }
    }
}
