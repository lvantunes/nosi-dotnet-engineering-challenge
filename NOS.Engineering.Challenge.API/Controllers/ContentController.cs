using System.Net;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    public ContentController(IContentsManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();

        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();

        return Ok(content);
    }

    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        return createdContent == null ? Problem() : Ok(createdContent);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        return Ok(deletedId);
    }

    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genres
    )
    {
        var contentToUpdate = await _manager.GetContent(id).ConfigureAwait(false);

        if (contentToUpdate == null)
        {
            return NotFound("Content not found.");
        }

        if (contentToUpdate.GenreList.Intersect(genres).Any())
        {
            return BadRequest("Cannot add repeated Genres.");
        }

        var genreList = contentToUpdate.GenreList.ToList();
        genreList.AddRange(genres);

        var newContent = new ContentInput() { Genres = genreList };

        var updatedContent = await _manager.UpdateContent(id, newContent.ToDto()).ConfigureAwait(false);

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
            return BadRequest("Genres list is empty or does not exist.");
        }

        var contentToUpdate = await _manager.GetContent(id).ConfigureAwait(false);

        if (contentToUpdate == null)
        {
            return NotFound("Content not found.");
        }

        if (!contentToUpdate.GenreList.Intersect(genres).Any())
        {
            return BadRequest("Genre not found.");
        }

        var genreList = contentToUpdate.GenreList.Except(genres);
        var newContent = new ContentInput() { Genres = genreList };

        var updatedContent = await _manager.UpdateContent(id, newContent.ToDto()).ConfigureAwait(false);

        return Ok(updatedContent);
    }
}