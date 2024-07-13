using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers.v2;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Tests.ControllerTests.v2;

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
        var filteredContentsResult = contentsResult.Where(x => x.Title == contentsResult.First().Title
                                                            || x.GenreList.Contains(contentsResult.First().GenreList.First()))
            .ToList();

        _manager.Setup(c => c.GetManyContents()).ReturnsAsync(contentsResult);
        _memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

        #endregion
        #region === Act ===

        var contents = await _contentController.GetManyContents(contentsResult.First().Title, contentsResult.First().GenreList.First());

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.IsType<OkObjectResult>(contents);

        var successResult = Assert.IsType<OkObjectResult>(contents);
        var createdContent = Assert.IsAssignableFrom<IEnumerable<Content>>(successResult.Value);

        Assert.Equal(createdContent, filteredContentsResult);

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

        var contents = await _contentController.GetManyContents(string.Empty, string.Empty);

        #endregion
        #region === Assert ===

        Assert.IsType<NotFoundResult>(contents);
        _manager.Verify(x => x.GetManyContents(), Times.Once);

        #endregion
    }

}
