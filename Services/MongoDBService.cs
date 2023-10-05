

using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_Code.Models;

namespace MongoDB_Code.Services
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class MongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoDBService(MyDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<BsonDocument>(settings.CollectionName);
        }

        public async Task<List<BsonDocument>> RetrieveDataAsync(int limit = 1)
        {
            try
            {
                var data = await _collection.Find(new BsonDocument()).ToListAsync();
                Console.WriteLine($"Retrieved {data.Count} documents from MongoDB.");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<BsonDocument>(); // Retorna uma lista vazia em vez de null
            }
        }

        public async Task<BsonDocument> RetrieveDataAsync(ObjectId? lastId)
        {
            try
            {
                var filter = lastId == null ? new BsonDocument() : Builders<BsonDocument>.Filter.Gt("_id", lastId);
                return await _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        private string GetDebuggerDisplay()
        {
            return ToString() ?? string.Empty;
        }
    }
}