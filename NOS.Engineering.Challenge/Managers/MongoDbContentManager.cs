using MongoDB.Driver;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class MongoDbContentManager : IContentsManager
{
    private readonly MongoDbContext _context;

    public MongoDbContentManager(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Content>> GetManyContents()
    {
        return await _context.Contents.Find(FilterDefinition<Content>.Empty).ToListAsync();
    }

    public async Task<Content> GetContent(Guid id)
    {
        return await _context.Contents.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Content> CreateContent(ContentDto contentDto)
    {
        var content = new Content(Guid.NewGuid(), contentDto);
        await _context.Contents.InsertOneAsync(content);
        return content;
    }

    public async Task<Content> UpdateContent(Guid id, ContentDto contentDto)
    {
        var content = new Content(id, contentDto);
        var result = await _context.Contents.ReplaceOneAsync(c => c.Id.Equals(id), content);
        return result.IsAcknowledged ? content : null;
    }

    public async Task<Guid> DeleteContent(Guid id)
    {
        var result = await _context.Contents.DeleteOneAsync(content => content.Id == id);
        return id;
    }
}
