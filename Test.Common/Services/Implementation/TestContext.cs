using System.Collections.Concurrent;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Services.Rest;

namespace Test.Common.Services.Implementation;

public sealed class TestContext : ITestContext
{
    private readonly IRestClientFactory _restClientFactory;

    public ConcurrentDictionary<int, User> CreatedUsers { get; }

    public ConcurrentDictionary<int, Post> CreatedPosts { get; }

    public ConcurrentDictionary<int, Comment> CreatedComments { get; }

    public TestContext(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;

        CreatedUsers = new ConcurrentDictionary<int, User>();
        CreatedPosts = new ConcurrentDictionary<int, Post>();
        CreatedComments = new ConcurrentDictionary<int, Comment>();
    }

    public async ValueTask DisposeAsync()
    {
        await CleanUpComments();
        await CleanUpPosts();
        await CleanUpUsers();
    }

    private async Task CleanUpUsers()
    {
        foreach (var (userId, _) in CreatedUsers)
        {
            var response = await _restClientFactory
                .SendRequestTo(HttpApisNames.ApiNames)
                .Delete($"{Endpoints.Users}/{userId}");
        }
    }

    private async Task CleanUpPosts()
    {
        foreach (var (postId, _) in CreatedPosts)
        {
            var response = await _restClientFactory
                .SendRequestTo(HttpApisNames.ApiNames)
                .Delete($"{Endpoints.Posts}/{postId}");
        }
    }

    private async Task CleanUpComments()
    {
        foreach (var (commentId, _) in CreatedComments)
        {
            var response = await _restClientFactory
                .SendRequestTo(HttpApisNames.ApiNames)
                .Delete($"{Endpoints.Comments}/{commentId}");
        }
    }
}
