using MongoDB.Bson.Serialization.Attributes;

namespace NOS.Engineering.Challenge.Models;

public class Content
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid Id { get; }
    public string Title { get; }
    public string SubTitle { get; }
    public string Description { get; }
    public string ImageUrl { get; }
    public int Duration { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public IEnumerable<string> GenreList { get; }


    public Content(Guid id, string title, string subTitle, string description, string imageUrl, int duration, DateTime startTime, DateTime endTime, IEnumerable<string> genreList)
    {
        Id = id;
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        GenreList = genreList;
    }

    public Content(Guid id , ContentDto contentDto)
    {
        Id = id;
        Title = contentDto.Title;
        SubTitle = contentDto.SubTitle;
        Description = contentDto.Description;
        ImageUrl = contentDto.ImageUrl;
        Duration = contentDto.Duration.Value;
        StartTime = contentDto.StartTime.Value;
        EndTime = contentDto.EndTime.Value;
        GenreList = contentDto.GenreList;
    }

    public ContentDto ToDto()
    {
        return new ContentDto(
            Title,
            SubTitle,
            Description,
            ImageUrl,
            Duration,
            StartTime,
            EndTime,
            GenreList
        );
    }
}