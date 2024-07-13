using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Controllers.v1;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Tests.ControllerTests.v1;

public class ContentControllerTests
{
    private Mock<IContentsManager> _manager;
    private Mock<IMemoryCache> _memoryCache;
    private Mock<ILogger<ContentController>> _logger;
    private IConfiguration _configuration;
    private ContentController _contentController;
    private Fixture _fixture;

    public ContentControllerTests()
    {
        _manager = new Mock<IContentsManager>();
        _memoryCache = new Mock<IMemoryCache>();
        _logger = new Mock<ILogger<ContentController>>();
        _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"Cache:TimeInMinutes", "15"},
                }).Build();
        _contentController = new ContentController(_manager.Object, _logger.Object, _memoryCache.Object, _configuration);
        _fixture = new Fixture();
    }

    [Fact]
    public void ConstructorNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ContentController(default, _logger.Object, _memoryCache.Object, _configuration));

        Assert.Throws<ArgumentNullException>(() => new ContentController(_manager.Object, default, _memoryCache.Object, _configuration));

        Assert.Throws<ArgumentNullException>(() => new ContentController(_manager.Object, _logger.Object, default, _configuration));

        Assert.Throws<ArgumentNullException>(() => new ContentController(_manager.Object, _logger.Object, _memoryCache.Object, default));
    }

    [Fact]
    public async Task GetManyContents_Success()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<IEnumerable<Content>>();

        _manager.Setup(c => c.GetManyContents()).ReturnsAsync(contentsResult);
        _memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

        #endregion
        #region === Act ===

        var contents = await _contentController.GetManyContents();

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<IEnumerable<Content>>(successResult.Value);
        Assert.Equal(createdContent, contentsResult);

        _manager.Verify(x => x.GetManyContents(), Times.Once);
        _memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task GetManyContents_NotFound()
    {
        #region === Arrange ===

        var contentsResult = Enumerable.Empty<Content>();

        _manager.Setup(c => c.GetManyContents()).ReturnsAsync(contentsResult);
        _memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

        #endregion
        #region === Act ===

        var contents = await _contentController.GetManyContents();

        #endregion
        #region === Assert ===

        Assert.IsType<NotFoundResult>(contents);
        _manager.Verify(x => x.GetManyContents(), Times.Once);

        #endregion
    }

    [Fact]
    public async Task GetContent_Success()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.GetContent(It.IsAny<Guid>());

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<Content>(successResult.Value);
        Assert.Equal(createdContent, contentsResult);

        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task GetContent_NotFound()
    {
        #region === Arrange ===

        var contentsResult = default(Content);

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.GetContent(It.IsAny<Guid>());

        #endregion
        #region === Assert ===

        Assert.IsType<NotFoundResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task CreateContent_Success()
    {
        #region === Arrange ===

        var contentResult = _fixture.Create<Content>();
        var contentInput = _fixture.Create<ContentInput>();

        _manager.Setup(c => c.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync(contentResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.CreateContent(contentInput);

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<Content>(successResult.Value);
        Assert.Equal(createdContent, contentResult);

        _manager.Verify(x => x.CreateContent(It.IsAny<ContentDto>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task CreateContent_Problem()
    {
        #region === Arrange ===

        var contentResult = default(Content);
        var contentInput = _fixture.Create<ContentInput>();

        _manager.Setup(c => c.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync(contentResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.CreateContent(contentInput);

        #endregion
        #region === Assert ===

        Assert.True(((IStatusCodeActionResult)contents).StatusCode == StatusCodes.Status500InternalServerError);
        _manager.Verify(x => x.CreateContent(It.IsAny<ContentDto>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task UpdateContent_Success()
    {
        #region === Arrange ===

        var contentResult = _fixture.Create<Content>();
        var contentInput = _fixture.Create<ContentInput>();

        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.UpdateContent(It.IsAny<Guid>(), contentInput);

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<Content>(successResult.Value);
        Assert.Equal(createdContent, contentResult);

        _manager.Verify(x => x.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task UpdateContent_NotFound()
    {
        #region === Arrange ===

        var contentResult = default(Content);
        var contentInput = _fixture.Create<ContentInput>();

        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.UpdateContent(It.IsAny<Guid>(), contentInput);

        #endregion
        #region === Assert ===

        Assert.IsType<NotFoundResult>(contents);
        _manager.Verify(x => x.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task DeleteContent_Success()
    {
        #region === Arrange ===

        var contentResult = _fixture.Create<Guid>();

        _manager.Setup(c => c.DeleteContent(It.IsAny<Guid>())).ReturnsAsync(contentResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.DeleteContent(It.IsAny<Guid>());

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);
        _manager.Verify(x => x.DeleteContent(It.IsAny<Guid>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task AddGenres_Success()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();
        var newGenreList = _fixture.Create<IEnumerable<string>>();

        var expectedGenreList = contentsResult.GenreList.ToList();
        expectedGenreList.AddRange(newGenreList.ToList());

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.AddGenres(It.IsAny<Guid>(), newGenreList);

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<Content>(successResult.Value);
        Assert.Equal(createdContent, contentsResult);

        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task AddGenres_NotFound()
    {
        #region === Arrange ===

        var contentsResult = default(Content);
        var newGenreList = _fixture.Create<IEnumerable<string>>();

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.AddGenres(It.IsAny<Guid>(), newGenreList);

        #endregion
        #region === Assert ===

        Assert.IsType<NotFoundObjectResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task AddGenres_BadRequest_EmptyInput()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();


        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.AddGenres(It.IsAny<Guid>(), It.Is<IEnumerable<string>>(x => x.Count() == 0)); // Pass the same genres to the method

        #endregion
        #region === Assert ===

        Assert.IsType<BadRequestObjectResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Never);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task AddGenres_BadRequest_DuplicateGenres()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();


        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.AddGenres(It.IsAny<Guid>(), contentsResult.GenreList); // Pass the same genres to the method

        #endregion
        #region === Assert ===

        Assert.IsType<BadRequestObjectResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task RemoveGenres_Success()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.RemoveGenres(It.IsAny<Guid>(), contentsResult.GenreList); // Pass the same genres to the method to successfuly remove

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<Content>(successResult.Value);
        Assert.Equal(createdContent, contentsResult);

        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Once);

        #endregion
    }

    [Fact]
    public async Task RemoveGenres_NotFound()
    {
        #region === Arrange ===

        var contentsResult = default(Content);
        var genresToRemove = _fixture.Create<IEnumerable<string>>();

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.RemoveGenres(It.IsAny<Guid>(), genresToRemove);

        #endregion
        #region === Assert ===

        Assert.IsType<NotFoundObjectResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task RemoveGenres_BadRequest_EmptyInput()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.RemoveGenres(It.IsAny<Guid>(), It.Is<IEnumerable<string>>(x => x.Count() == 0)); // Pass the same genres to the method

        #endregion
        #region === Assert ===

        Assert.IsType<BadRequestObjectResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Never);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task RemoveGenres_BadRequest_GenresDoNotExist()
    {
        #region === Arrange ===

        var contentsResult = _fixture.Create<Content>();
        var genresToRemove = _fixture.Create<IEnumerable<string>>();

        _manager.Setup(c => c.GetContent(It.IsAny<Guid>())).ReturnsAsync(contentsResult);
        _manager.Setup(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(contentsResult);

        #endregion
        #region === Act ===

        var contents = await _contentController.RemoveGenres(It.IsAny<Guid>(), genresToRemove);

        #endregion
        #region === Assert ===

        Assert.IsType<BadRequestObjectResult>(contents);
        _manager.Verify(x => x.GetContent(It.IsAny<Guid>()), Times.Once);
        _manager.Verify(c => c.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()), Times.Never);

        #endregion
    }
}
