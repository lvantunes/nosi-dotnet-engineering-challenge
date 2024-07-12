using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoDbClient = new MongoClient(mongoDbSettings.Value.ConnectionUri);
        _database = mongoDbClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
    }

    public IMongoCollection<Content> Contents => _database.GetCollection<Content>("Content");

    public class MongoDbSettings
    {
        public string ConnectionUri { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
