using System;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.Utils;

namespace Test.Common.Services.Implementation;

public class UserService : IUserService
{
    private readonly IRestClientFactory _http;
    private readonly ITestContext _context;

    public UserService(
        IRestClientFactory httpClientFactory,
        ITestContext context)
    {
        _http = httpClientFactory;
        _context = context;
    }

    public Task<Response<User>?> GetUserAsync(int userId)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Users}/{userId}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<User>>();
    }

    public Task<CollectionResponse<User>?> GetUsersFromPageAsync(int pageNumber)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Users + $"?page={pageNumber}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<User>>();
    }

    public Task<CollectionResponse<User>?> GetUsersAsync()
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Users)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<User>>();
    }

    public async Task<Response<User>?> CreateUserAsync(User user)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post(Endpoints.Users, user)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<User>>();

        _context.CreatedUsers.TryAdd(result!.Data.Id, result.Data);

        return result;
    }

    public Task<Response<User>?> UpdateUserAsync(int userId, User user)
    {
        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Users}/{userId}", user)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<User>>();
    }

    public Task<Response<User>?> PartialUpdateUserAsync(int userId, Action<UserPatchRequest> patch)
    {
        var patchRequest = new UserPatchRequest();
        patch(patchRequest);

        return _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch(Endpoints.Users + "/" + userId, patchRequest)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<User>>();
    }

    public async Task<RestResponse> DeleteUserAsync(int userId)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Delete(Endpoints.Users + "/" + userId)
            .EnsureSuccessStatusCodeAsync();

        _context.CreatedUsers.TryRemove(userId, out _);

        return result;
    }

    public async Task<Response<Post>?> CreatePostToUserAsync(int userId, Post post)
    {
        var result = await _http
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post($"{Endpoints.Users}/{userId}/posts", post)
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<Response<Post>>();

        _context.CreatedPosts.TryAdd(result!.Data.Id, result.Data);

        return result;
    }
}
