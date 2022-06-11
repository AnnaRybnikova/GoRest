using System;
using System.Threading.Tasks;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;

namespace Test.Common.Services;

public interface ICommentService
{
    Task<Response<Comment>?> GetCommentAsync(int commentId);

    Task<CollectionResponse<Comment>?> GetCommentsAsync();

    Task<CollectionResponse<Comment>?> GetCommentsFromPageAsync(int pageNumber);

    Task<Response<Comment>?> CreateCommentAsync(Comment comment);

    Task<Response<Comment>?> UpdateCommentAsync(int commentId, Comment comment);

    Task<Response<Comment>?> PartialUpdateCommentAsync(int commentId, Action<CommentPatchRequest> patch);

    Task<RestResponse> DeleteCommentAsync(int commentId);
}
