using System;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;

namespace Test.Common.Services;

public interface IPostService
{
    Task<Response<Post>?> GetPostAsync(int postId);

    Task<CollectionResponse<Post>?> GetPostsAsync();

    Task<CollectionResponse<Post>?> GetPostsFromPageAsync(int pageNumber);

    Task<Response<Post>?> CreatePostAsync(Post post);

    Task<Response<Post>?> UpdatePostAsync(int postId, Post post);

    Task<Response<Post>?> PartialUpdatePostAsync(int postId, Action<PostPatchRequest> patch);

    Task<RestResponse> DeletePostAsync(int postId);

    Task<Response<Comment>?> CreateCommentToPostAsync(int postId, Comment comment);
}
