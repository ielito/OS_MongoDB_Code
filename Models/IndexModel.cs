using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB_Code.Services;

namespace MongoDB_Code.Models
{
    public class IndexModel
    {
        private readonly MongoDBService _mongoDBService;

        public IndexModel(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public BsonDocument? Document { get; set; }

        public async Task RetrieveDataAsync()
        {
            var documents = await _mongoDBService.RetrieveDataAsync();
            Document = documents.FirstOrDefault();
        }
    }
}
