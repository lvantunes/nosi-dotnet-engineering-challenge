using AutoFixture;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Tests.Managers;

public class ContentsManagerTests
{
    private Fixture _fixture;
    private ContentsManager _contentManager;
    private Mock<IDatabase<Content, ContentDto>> _database;

    public ContentsManagerTests()
    {
        _fixture = new Fixture();
        _database = new Mock<IDatabase<Content, ContentDto>>();
        _contentManager = new ContentsManager(_database.Object);
    }

    [Fact]
    public async Task GetManyContents_Success()
    {
        #region === Arrange ===

        var content = _fixture.Create<IEnumerable<Content>>();

        _database.Setup(x => x.ReadAll()).ReturnsAsync(content);


        #endregion
        #region === Act ===

        var contentToReturn = await _contentManager.GetManyContents();

        #endregion
        #region === Assert ===

        Assert.NotNull(contentToReturn);
        Assert.Equal(contentToReturn, content);

        #endregion
    }

    [Fact]
    public async Task CreateContent_Success()
    {
        #region === Arrange ===

        var content = _fixture.Create<Content>();
        var contentDto = _fixture.Create<ContentDto>();

        _database.Setup(x => x.Create(It.IsAny<ContentDto>())).ReturnsAsync(content);


        #endregion
        #region === Act ===

        var contentToReturn = await _contentManager.CreateContent(contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(contentToReturn);
        Assert.Equal(contentToReturn, content);

        #endregion
    }

    [Fact]
    public async Task GetContent_Success()
    {
        #region === Arrange ===

        var content = _fixture.Create<Content>();
        var contentDto = _fixture.Create<ContentDto>();

        _database.Setup(x => x.Read(It.IsAny<Guid>())).ReturnsAsync(content);


        #endregion
        #region === Act ===

        var contentToReturn = await _contentManager.GetContent(Guid.NewGuid());

        #endregion
        #region === Assert ===

        Assert.NotNull(contentToReturn);
        Assert.Equal(contentToReturn, content);

        #endregion
    }

    [Fact]
    public async Task UpdateContent_Success()
    {
        #region === Arrange ===

        var content = _fixture.Create<Content>();
        var contentDto = _fixture.Create<ContentDto>();

        _database.Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(content);


        #endregion
        #region === Act ===

        var contentToReturn = await _contentManager.UpdateContent(Guid.NewGuid(), contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(contentToReturn);
        Assert.Equal(contentToReturn, content);

        #endregion
    }

    [Fact]
    public async Task DeleteContent_Success()
    {
        #region === Arrange ===

        var content = Guid.NewGuid();

        _database.Setup(x => x.Delete(It.IsAny<Guid>())).ReturnsAsync(content);


        #endregion
        #region === Act ===

        var contentToReturn = await _contentManager.DeleteContent(Guid.NewGuid());

        #endregion
        #region === Assert ===

        Assert.Equal(contentToReturn, content);

        #endregion
    }
}
