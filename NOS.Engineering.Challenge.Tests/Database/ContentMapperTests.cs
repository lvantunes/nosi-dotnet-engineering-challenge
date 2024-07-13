using AutoFixture;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Tests.Database;

public class ContentMapperTests
{
    private Fixture _fixture;
    private ContentMapper _contentMapper;

    public ContentMapperTests()
    {
        _fixture = new Fixture();
        _contentMapper = new ContentMapper();
    }

    [Fact]
    public void Map_Success()
    {
        #region === Arrange ===

        var contentDto = _fixture.Create<ContentDto>();


        #endregion
        #region === Act ===

        var contents = _contentMapper.Map(It.IsAny<Guid>(), contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(contents);
        Assert.Equal(contents.Title, contentDto.Title);
        Assert.Equal(contents.SubTitle, contentDto.SubTitle);
        Assert.Equal(contents.Description, contentDto.Description);
        Assert.Equal(contents.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(contents.Duration, contentDto.Duration);
        Assert.Equal(contents.StartTime, contentDto.StartTime);
        Assert.Equal(contents.EndTime, contentDto.EndTime);
        Assert.Equal(contents.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Map_Fail_Title_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.Title, default(string))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Map_Fail_SubTitle_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.SubTitle, default(string))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Map_Fail_Description_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.Description, default(string))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Map_Fail_ImageUrl_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.ImageUrl, default(string))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Map_Fail_Duration_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.Duration, default(int?))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Map_Fail_StartTime_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.StartTime, default(DateTime?))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Map_Fail_EndTime_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.EndTime, default(DateTime?))
            .Create();

        #endregion

        #region === Act & Assert ===

        Assert.Throws<ArgumentNullException>(() => _contentMapper.Map(It.IsAny<Guid>(), contentDto));

        #endregion
    }

    [Fact]
    public void Patch_Success()
    {
        #region === Arrange ===

        var contentDto = _fixture.Create<ContentDto>();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(patchedContent);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_Title_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.Title, default(string?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, content.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_SubTitle_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.SubTitle, default(string?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, content.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_Description_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.Description, default(string?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, content.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_ImageUrl_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.ImageUrl, default(string?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, content.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_Duration_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.Duration, default(int?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, content.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_StartTime_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.StartTime, default(DateTime?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, content.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_EndTime_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.EndTime, default(DateTime?))
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, content.EndTime);
        Assert.Equal(patchedContent.GenreList, contentDto.GenreList);

        #endregion
    }

    [Fact]
    public void Patch_Success_Genres_Empty()
    {
        #region === Arrange ===

        var contentDto = _fixture.Build<ContentDto>()
            .With(x => x.GenreList, Enumerable.Empty<string>())
            .Create();
        var content = _fixture.Create<Content>();

        #endregion
        #region === Act ===

        var patchedContent = _contentMapper.Patch(content, contentDto);

        #endregion
        #region === Assert ===

        Assert.NotNull(content);
        Assert.Equal(patchedContent.Title, contentDto.Title);
        Assert.Equal(patchedContent.SubTitle, contentDto.SubTitle);
        Assert.Equal(patchedContent.Description, contentDto.Description);
        Assert.Equal(patchedContent.ImageUrl, contentDto.ImageUrl);
        Assert.Equal(patchedContent.Duration, contentDto.Duration);
        Assert.Equal(patchedContent.StartTime, contentDto.StartTime);
        Assert.Equal(patchedContent.EndTime, contentDto.EndTime);
        Assert.Equal(patchedContent.GenreList, content.GenreList);

        #endregion
    }

}
