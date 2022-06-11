using Bogus;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Enums;

namespace Test.Common.Utils;

public static class FakeData
{
    private static string _letters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";

    private static readonly Faker<User> _userFaker;

    private static readonly Faker<Post> _postFaker;

    private static readonly Faker<Comment> _commentFaker;

    static FakeData()
    {
        _userFaker = new Faker<User>()
            .RuleFor(x => x.Gender, f
                => f.Person.Gender == Bogus.DataSets.Name.Gender.Male ? Gender.Male : Gender.Female)
            .RuleFor(x => x.Name, f =>
            {
                return f.Person.FirstName + f.Person.LastName;
            })
            .RuleFor(x => x.Email, f =>
            {
                return f.Internet.Email(
                    firstName: f.Person.FirstName,
                    lastName: $"{f.Person.LastName}{f.Random.Word()}",
                    provider: "gmail.com");
            })
            .RuleFor(x => x.Status, f => f.Random.Enum<Status>());

        _postFaker = new Faker<Post>()
            .RuleFor(x => x.Title, f => f.Random.String2(8, _letters))
            .RuleFor(x => x.Body, f => f.Random.Words(20));

        _commentFaker = new Faker<Comment>()
            .RuleFor(x => x.Email, f =>
            {
                return f.Internet.Email(
                    firstName: f.Person.FirstName,
                    lastName: $"{f.Person.LastName}{f.Random.Word()}",
                    provider: "gmail.com");
            })
            .RuleFor(x => x.Name, f => f.Random.String2(8, _letters))
            .RuleFor(x => x.Body, f => f.Random.Words(10));
    }

    public static User CreateValidUser() => _userFaker.Generate();

    public static Post CreateValidPost() => _postFaker.Generate();

    public static Comment CreateValidComment() => _commentFaker.Generate();

    public static User CreateEmptyUser() => new()
    {
        Name = string.Empty,
        Email = string.Empty
    };

    public static Post CreateEmptyPost() => new()
    {
        Body = string.Empty,
        Title = string.Empty
    };

    public static Comment CreateEmptyComment() => new()
    {
        Email = string.Empty,
        Name = string.Empty,
        Body = string.Empty,
    };
}
