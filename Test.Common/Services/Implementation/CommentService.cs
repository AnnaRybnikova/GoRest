using System;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.Utils;

namespace Test.Common.Services.Implementation;

public class CommentService : ICommentService
{
    private readonly IRestClientFactory _http;
    private readonly ITestContext _context;

    public CommentService(
        IRestClientFactory httpClientFactory,
        ITestContext context)
    {
        _http = httpClientFactory;
        _context = context;
    }

    public Task<Response<Comment>?> GetCommentAsync(int commentId)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Comments}/{commentId}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Comment>>();
    }

    public Task<CollectionResponse<Comment>?> GetCommentsAsync()
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Comments)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();
    }

    public Task<CollectionResponse<Comment>?> GetCommentsFromPageAsync(int pageNumber)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Comments + $"?page={pageNumber}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();
    }

    public async Task<Response<Comment>?> CreateCommentAsync(Comment comment)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post($"{Endpoints.Comments}", comment)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Comment>>();

        _context.CreatedComments.TryAdd(result!.Data.Id, result.Data);

        return result;
    }

    public Task<Response<Comment>?> UpdateCommentAsync(int commentId, Comment comment)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Comments}/{commentId}", comment)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Comment>>();
    }

    public Task<Response<Comment>?> PartialUpdateCommentAsync(int commentId, Action<CommentPatchRequest> patch)
    {
        var patchRequest = new CommentPatchRequest();
        patch(patchRequest);

        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch(Endpoints.Comments + "/" + commentId, patchRequest)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Comment>>();
    }

    public async Task<RestResponse> DeleteCommentAsync(int commentId)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Delete(Endpoints.Comments + "/" + commentId)
            .EnsureSuccessStatusCodeAsync();

        _context.CreatedComments.TryRemove(commentId, out _);

        return result;
    }
}
