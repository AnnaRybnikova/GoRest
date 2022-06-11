using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Common;
using Test.Common.Domain.Data;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.TestData;
using Test.Common.Utils;

namespace Tests.Integration.Tests.Positive;

public class ComplexTest : TestBase
{
    [Category("ComplexTestPost")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ComplexTestPosts))]
    public async Task Create21UniquePost_FindPostAndCheckMetaData_Successful(TestData testData)
    {
        // Arrange
        List<int> postId = new();

        var createUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createUser!.Data.Id;

        foreach (var post in testData.Posts)
        {
            var createPost = await Services.Users().CreatePostToUserAsync(userId, post);
            
            postId.Add(createPost!.Data.Id);
            
            await Task.Delay(1000);
        }

        // Act
        var allUserPosts = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Users}/{userId}/posts")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();

        var pageTwoUserPosts = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Users}/{userId}/posts?page=2")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();

        var postIdFromList = postId[5];

        var postByIdFromList = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Users}/{userId}/posts?id={postId[5]}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();

        foreach (var id in postId)
        {
            _ = await Services.Posts().DeletePostAsync(id);
        }

        postId.Clear();

        var allRemainingUserPosts = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Users}/{userId}/posts")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(allUserPosts!.Data[0].UserId,
                Is.EqualTo(userId),
                $"Expected that UserId of first post is {userId}, " +
                $"but received {allUserPosts.Data[0].UserId} ");

            Assert.That(allUserPosts.Data[0].UserId,
                Is.EqualTo(allUserPosts.Data[19].UserId),
                $"Expected that UserId is {allUserPosts.Data[19].UserId}, " +
                $"but received {allUserPosts.Data[0].UserId} ");

            Assert.That(allUserPosts.Meta.Pagination.Total,
                Is.EqualTo(21),
                "Expected that Total number of received results is 21, " +
                $"but received {allUserPosts.Meta.Pagination.Total} ");

            Assert.That(allUserPosts.Meta.Pagination.Pages,
                Is.EqualTo(2),
                "Expected that total number of Pages is 2, " +
                $"but received {allUserPosts.Meta.Pagination.Pages} ");

            Assert.That(pageTwoUserPosts!.Meta.Pagination.Page,
                Is.EqualTo(2),
                "Expected that current metaData.Page parameter of second page request is 2, " +
                $"but received {pageTwoUserPosts.Meta.Pagination.Page} ");

            Assert.That(pageTwoUserPosts.Data.Length,
                Is.EqualTo(1),
                "Expected that there is only 1 post on the second page, " +
                $" but received {pageTwoUserPosts.Data.Length} posts");

            Assert.That(postByIdFromList!.Meta.Pagination.Total,
                Is.EqualTo(1),
                "Expected that Total number of posts by Id is 1, " +
                $"but received {postByIdFromList.Meta.Pagination.Total} posts with id = {postIdFromList}");

            Assert.That(postByIdFromList.Meta.Pagination.Pages,
                Is.EqualTo(1),
                "Expected that Total pages of requesting posts by Id is 1, " +
                $"but received {postByIdFromList.Meta.Pagination.Pages} pages of posts with id = {postIdFromList}");

            Assert.That(postByIdFromList.Data[0].Id,
                Is.EqualTo(postIdFromList),
                $"Expected that post Id is {postIdFromList}, " +
                $"but received {postByIdFromList.Data[0].Id} ");

            Assert.That(allRemainingUserPosts!.Meta.Pagination.Total,
                Is.EqualTo(0),
                "Expected that there is no posts Pages to receive after deleting, " +
                $"but received {allRemainingUserPosts.Meta.Pagination.Total}, so posts deleting failed");

            Assert.That(allRemainingUserPosts.Data.Length,
                Is.EqualTo(0),
                "Expected that there is no posts to receive after deleting, " +
                $"but received {allRemainingUserPosts.Data.Length}, so posts deleting failed");
        });

    }

    [Category("ComplexTestComment")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ComplexTestComments))]
    public async Task Create21UniqueComment_FindCommentAndCheckMetaData_Successful(TestData testData)
    {
        // Arrange
        List<int> commentId = new();

        var createUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createUser!.Data.Id;

        var createPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createPost!.Data.Id;

        foreach (var comment in testData.Comments)
        {
            var createComment = await Services.Posts()
                .CreateCommentToPostAsync(postId, comment);
            
            commentId.Add(createComment!.Data.Id);
            
            await Task.Delay(1000);
        }

        // Act
        var allPostComments = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Posts}/{postId}/comments")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();

        var pageTwoPostComments = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Posts}/{postId}/comments?page=2")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();

        var commentIdFromList = commentId[5];

        var commentByIdFromList = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Posts}/{postId}/comments?id={commentId[5]}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();

        foreach (var id in commentId)
        {
            _ = await Services.Comments().DeleteCommentAsync(id);
        }

        commentId.Clear();

        var allRemainingPostComments = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Posts}/{postId}/comments")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(allPostComments!.Data[0].PostId,
                Is.EqualTo(postId),
                $"Expected that PostId of first comment is {postId}, " +
                $"but received {allPostComments.Data[0].PostId} ");

            Assert.That(allPostComments.Data[0].PostId,
                Is.EqualTo(allPostComments.Data[19].PostId),
                $"Expected that PostId is {allPostComments.Data[19].PostId}, " +
                $"but received {allPostComments.Data[0].PostId} ");

            Assert.That(allPostComments.Meta.Pagination.Total,
                Is.EqualTo(21),
                "Expected that Total number of received results is 21, " +
                $"but received {allPostComments.Meta.Pagination.Total} ");

            Assert.That(allPostComments.Meta.Pagination.Pages,
                Is.EqualTo(2),
                "Expected that total number of Pages is 2, " +
                $"but received {allPostComments.Meta.Pagination.Pages} ");

            Assert.That(pageTwoPostComments!.Meta.Pagination.Page,
                Is.EqualTo(2),
                "Expected that current metaData.Page parameter of second page request is 2, " +
                $"but received {pageTwoPostComments.Meta.Pagination.Page} ");

            Assert.That(pageTwoPostComments.Data.Length,
                Is.EqualTo(1),
                "Expected that there is only 1 comment on the second page, " +
                $" but received {pageTwoPostComments.Data.Length} comments");

            Assert.That(commentByIdFromList!.Meta.Pagination.Total,
                Is.EqualTo(1),
                "Expected that Total number of comments by Id is 1, " +
                $"but received {commentByIdFromList.Meta.Pagination.Total} comments " +
                $"with id = {commentIdFromList}");

            Assert.That(commentByIdFromList.Meta.Pagination.Pages,
                Is.EqualTo(1),
                "Expected that Total pages of requesting comments by Id is 1, " +
                $"but received {commentByIdFromList.Meta.Pagination.Pages} pages of comments " +
                $"with id = {commentIdFromList}");

            Assert.That(commentByIdFromList.Data[0].Id,
                Is.EqualTo(commentIdFromList),
                $"Expected that comment Id is {commentIdFromList}, " +
                $"but received {commentByIdFromList.Data[0].Id} ");

            Assert.That(allRemainingPostComments!.Meta.Pagination.Total,
                Is.EqualTo(0),
                "Expected that there is no comments Pages to receive after deleting, " +
                $"but received {allRemainingPostComments.Meta.Pagination.Total}, so comments deleting failed");

            Assert.That(allRemainingPostComments.Data.Length,
                Is.EqualTo(0),
                "Expected that there is no comments to receive after deleting, " +
                $"but received {allRemainingPostComments.Data.Length}, so comments deleting failed");
        });
    }
}
