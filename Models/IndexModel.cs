using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public List<BsonDocument> Documents { get; private set; } = new List<BsonDocument>();

        public async Task RetrieveDataAsync(int? limit)
        {
            Documents = await _mongoDBService.RetrieveDataAsync(limit ?? 1);
        }
    }
}