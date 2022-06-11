using System;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.Utils;

namespace Test.Common.Services.Implementation;

public class PostService : IPostService
{
    private readonly IRestClientFactory _http;
    private readonly ITestContext _context;

    public PostService(
        IRestClientFactory httpClientFactory,
        ITestContext context)
    {
        _http = httpClientFactory;
        _context = context;
    }

    public Task<Response<Post>?> GetPostAsync(int postId)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Posts + $"/{postId}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Post>>();
    }

    public Task<CollectionResponse<Post>?> GetPostsAsync()
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Posts)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();
    }

    public Task<CollectionResponse<Post>?> GetPostsFromPageAsync(int pageNumber)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Posts + $"?page={pageNumber}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();
    }

    public async Task<Response<Post>?> CreatePostAsync(Post post)
    {
        var result = await _http
           .SendRequestTo(HttpApisNames.ApiNames)
           .Post($"{Endpoints.Posts}", post)
           .EnsureSuccessStatusCodeAsync()
           .ReadAsync<Response<Post>>();

        _context.CreatedPosts.TryAdd(result!.Data.Id, result.Data);

        return result;
    }

    public Task<Response<Post>?> UpdatePostAsync(int postId, Post post)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Posts}/{postId}", post)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Post>>();
    }

    public Task<Response<Post>?> PartialUpdatePostAsync(int postId, Action<PostPatchRequest> patch)
    {
        var patchRequest = new PostPatchRequest();
        patch(patchRequest);

        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch(Endpoints.Posts + "/" + postId, patchRequest)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Post>>();
    }
    public async Task<RestResponse> DeletePostAsync(int postId)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Delete(Endpoints.Posts + "/" + postId)
            .EnsureSuccessStatusCodeAsync();

        _context.CreatedPosts.TryRemove(postId, out _);

        return result;
    }

    public async Task<Response<Comment>?> CreateCommentToPostAsync(int postId, Comment comment)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post($"{Endpoints.Posts}/{postId}/comments", comment)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Comment>>();

        _context.CreatedComments.TryAdd(result!.Data.Id, result.Data);

        return result;
    }
}
