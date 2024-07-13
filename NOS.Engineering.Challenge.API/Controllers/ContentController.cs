using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;
    private readonly IMemoryCache _cache;

    private const string CACHE_KEY = "Nos_Challenge_Contents_{0}";
    private readonly int _cacheTimeInMinutes;

    public ContentController(IContentsManager manager, ILogger<ContentController> logger, IMemoryCache cache, IConfiguration configuration)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheTimeInMinutes = configuration.GetValue<int>("Cache:TimeInMinutes");
    }

    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        _logger.LogInformation($"[{DateTime.UtcNow}]: Getting contents");
        if (_cache.TryGetValue(string.Format(CACHE_KEY, "GetManyContents"), out IEnumerable<Content> contents))
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Contents fetched successfuly");
            return Ok(contents);
        }

        contents = await _manager.GetManyContents().ConfigureAwait(false);
        _cache.Set(string.Format(CACHE_KEY, "GetManyContents"), contents, TimeSpan.FromMinutes(_cacheTimeInMinutes));

        if (!contents.Any())
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: No contents found");
            return NotFound();
        }
        _logger.LogInformation($"[{DateTime.UtcNow}]: Contents fetched successfuly");
        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation($"[{DateTime.UtcNow}]: Getting content with Id {{{id}}}");
        if (_cache.TryGetValue(string.Format(CACHE_KEY, "GetManyContents"), out IEnumerable<Content> contents))
        {
            if (contents.Where(x => x.Id ==id).FirstOrDefault() == default)
            {
                _logger.LogInformation($"[{DateTime.UtcNow}]: Content not found");
                return NotFound();
            }

            return Ok(contents.Where(x => x.Id == id).FirstOrDefault());
        }

        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Content not found");
            return NotFound();
        }
        _logger.LogInformation($"[{DateTime.UtcNow}]: Content fetched successfuly");
        return Ok(content);
    }

    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation($"[{DateTime.UtcNow}]: Creating a new content");
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        if (createdContent == null)
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Could not create a new content");
            return Problem();
        }

        _cache.Remove(string.Format(CACHE_KEY, "GetManyContents")); // Force update on next fetch

        _logger.LogInformation($"[{DateTime.UtcNow}]: New content created with Id {{{createdContent.Id}}}");

        return Ok(createdContent);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation($"[{DateTime.UtcNow}]: Updating content with Id {{{id}}}");

        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        if (updatedContent == null)
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Could update content");
            return NotFound();
        }
        _cache.Remove(string.Format(CACHE_KEY, "GetManyContents")); // Force update on next fetch
        _logger.LogInformation($"[{DateTime.UtcNow}]: Content with Id {{{id}}} updated successfuly");
        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        _logger.LogInformation($"[{DateTime.UtcNow}]: Deleting content with Id {{{id}}}");

        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        _cache.Remove(string.Format(CACHE_KEY, "GetManyContents")); // Force update on next fetch
        return Ok(deletedId);
    }

    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genres
    )
    {
        if (genres == null || !genres.Any())
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Inputed genres list is empty");
            return BadRequest("Genres list is empty or does not exist.");
        }
        _logger.LogInformation($"[{DateTime.UtcNow}]: Adding gernes to content with Id {{{id}}}");
        var contentToUpdate = await _manager.GetContent(id).ConfigureAwait(false);

        if (contentToUpdate == null)
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Content not found");
            return NotFound("Content not found.");
        }

        if (contentToUpdate.GenreList.Intersect(genres).Any())
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Cannot add repetead genres");
            return BadRequest("Cannot add repeated Genres.");
        }

        var genreList = contentToUpdate.GenreList.ToList();
        genreList.AddRange(genres);

        var newContent = new Content(contentToUpdate.Id,
                                     contentToUpdate.Title,
                                     contentToUpdate.SubTitle,
                                     contentToUpdate.Description,
                                     contentToUpdate.ImageUrl,
                                     contentToUpdate.Duration,
                                     contentToUpdate.StartTime,
                                     contentToUpdate.EndTime,
                                     genreList);

        var updatedContent = await _manager.UpdateContent(id, newContent.ToDto()).ConfigureAwait(false);
        _cache.Remove(string.Format(CACHE_KEY, "GetManyContents")); // Force update on next fetch
        _logger.LogInformation($"[{DateTime.UtcNow}]: Content updated successfuly");
        return Ok(updatedContent);
    }

    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genres
    )
    {
        if (genres == null || !genres.Any())
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Inputed genres list is empty");
            return BadRequest("Genres list is empty or does not exist.");
        }
        _logger.LogInformation($"[{DateTime.UtcNow}]: Removing gernes to content with Id {{{id}}}");
        var contentToUpdate = await _manager.GetContent(id).ConfigureAwait(false);

        if (contentToUpdate == null)
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Content not found");
            return NotFound("Content not found.");
        }

        if (!contentToUpdate.GenreList.Intersect(genres).Any())
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: Inputed genres were not found");
            return BadRequest("Genre not found.");
        }

        var genreList = contentToUpdate.GenreList.Except(genres);
        var newContent = new Content(contentToUpdate.Id,
                                     contentToUpdate.Title,
                                     contentToUpdate.SubTitle,
                                     contentToUpdate.Description,
                                     contentToUpdate.ImageUrl,
                                     contentToUpdate.Duration,
                                     contentToUpdate.StartTime,
                                     contentToUpdate.EndTime,
                                     genreList);

        var updatedContent = await _manager.UpdateContent(id, newContent.ToDto()).ConfigureAwait(false);
        _cache.Remove(string.Format(CACHE_KEY, "GetManyContents")); // Force update on next fetch
        _logger.LogInformation($"[{DateTime.UtcNow}]: Content updated successfuly");
        return Ok(updatedContent);
    }
}