using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers.v2;


[ApiVersion(2.0)]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
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
    public async Task<IActionResult> GetManyContents([FromQuery] string title = "", string genre = "")
    {
        _logger.LogInformation($"[{DateTime.UtcNow}]: Getting contents");
        if (_cache.TryGetValue(string.Format(CACHE_KEY, "GetManyContents"), out IEnumerable<Content> contents))
        {
            contents = contents.Where(x => x.Title == title || x.GenreList.Contains(genre)).ToList();

            if (!contents.Any())
            {
                _logger.LogInformation($"[{DateTime.UtcNow}]: No contents found");
                return NotFound();
            }

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

        var contentToReturn = contents.Where(x => x.Title.Contains(title) || x.GenreList.Contains(genre)).ToList();

        if (!contentToReturn.Any())
        {
            _logger.LogInformation($"[{DateTime.UtcNow}]: No contents found");
            return NotFound();
        }

        return Ok(contentToReturn);
    }

}