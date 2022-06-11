using System;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;

namespace Test.Common.Services;

public interface IUserService
{
    Task<Response<User>?> GetUserAsync(int userId);

    Task<CollectionResponse<User>?> GetUsersAsync();

    Task<CollectionResponse<User>?> GetUsersFromPageAsync(int pageNumber);

    Task<Response<User>?> CreateUserAsync(User user);

    Task<Response<User>?> UpdateUserAsync(int userId, User user);

    Task<Response<User>?> PartialUpdateUserAsync(int userId, Action<UserPatchRequest> patch);

    Task<RestResponse> DeleteUserAsync(int userId);

    Task<Response<Post>?> CreatePostToUserAsync(int userId, Post post);
}
