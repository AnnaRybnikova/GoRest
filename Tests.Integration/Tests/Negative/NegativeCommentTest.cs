using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Common;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.Data.Responses;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.TestData;

namespace Tests.Integration.Tests.Negative;

internal class NegativeCommentTest : TestBase
{
    // Without Authorization

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreateComment_Unauthorized(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var createdComment = await TestServices
           .UnauthorizedRestClientFactory
           .SendRequestTo(HttpApisNames.ApiNames)
           .Post($"{Endpoints.Posts}/{postId}/comments", testData.Comment.ForCreate);

        // Assert
        Assert.That(createdComment.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.Unauthorized),
            $"The received StatusCode {createdComment.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.Unauthorized}.");
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task GetComment_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        // Act
        var receivedComment = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Comments + "/" + commentId);

        //Assert
        Assert.That(receivedComment.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {receivedComment.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Delete")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task DeleteComment_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        // Act
        var deleteComment = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Delete(Endpoints.Comments + "/" + commentId);

        // Assert
        Assert.IsTrue(deleteComment.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound,
            $"The received StatusCode {deleteComment.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task PartiallyUpdateComment_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        // Act
        var updatedComment = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch(Endpoints.Comments + "/" + commentId, testData.Post.ForUpdate);

        // Assert
        Assert.That(updatedComment.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {updatedComment.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdateComment_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        // Act
        var updatedComment = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put(Endpoints.Comments + "/" + commentId, testData.Post.ForUpdate);

        // Assert
        Assert.IsTrue(updatedComment.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound,
            $"The received StatusCode {updatedComment.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    // Incorrect Data

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidData))]
    public async Task CreateComment_WithInvalidData_ReturnsUnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var createdComment = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post($"{Endpoints.Posts}/{postId}/comments", testData.InvalidComment.ForCreate);

        var errorResponse = await createdComment.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createdComment.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} but received " +
                $"{createdComment.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Field == "name"
                          && errorResponse.Data[0].Message == "can't be blank",
                "The field and message is incorrect. Expected name : can't be blank " +
                $"but received {errorResponse.Data[0].Field} : " +
                $"{errorResponse.Data[0].Message}");

            Assert.IsTrue(errorResponse.Data[1].Field == "email"
                          && errorResponse.Data[1].Message == "can't be blank, is invalid",
                "The field and message is incorrect. Expected email : can't be blank, is invalid " +
                $"but received {errorResponse.Data[1].Field} : " +
                $"{errorResponse.Data[1].Message}");

            Assert.IsTrue(errorResponse.Data[2].Field == "body"
                          && errorResponse.Data[2].Message == "can't be blank",
                "The field and message is incorrect. Expected body : can't be blank " +
                $"but received {errorResponse.Data[2].Field} : " +
                $"{errorResponse.Data[2].Message}");

            Assert.IsNull(errorResponse.Meta,
                "If the request is incorrect, the Meta data must be null, " +
                $"but received {errorResponse.Meta}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidData))]
    public async Task UpdateComment_WithInvalidData_ReturnsUnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        // Act
        var updateComment = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Comments}/{commentId}", testData.InvalidComment.ForUpdate);

        var errorResponse = await updateComment.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updateComment.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} " +
                $"but received {updateComment.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Field == "post"
                          && errorResponse.Data[0].Message == "must exist",
                "The field and message is incorrect. Expected user : must exist " +
                $"but received {errorResponse.Data[0].Field} : " +
                $"{errorResponse.Data[0].Message}");

            Assert.IsTrue(errorResponse.Data[1].Field == "name"
                          && errorResponse.Data[1].Message == "can't be blank",
                "The field and message is incorrect. Expected name : can't be blank " +
                $"but received {errorResponse.Data[1].Field} : " +
                $"{errorResponse.Data[1].Message}");

            Assert.IsTrue(errorResponse.Data[2].Field == "email"
                          && errorResponse.Data[2].Message == "can't be blank, is invalid",
                "The field and message is incorrect. Expected email : can't be blank, is invalid " +
                $"but received {errorResponse.Data[2].Field} : " +
                $"{errorResponse.Data[2].Message}");

            Assert.IsTrue(errorResponse.Data[3].Field == "body"
                          && errorResponse.Data[3].Message == "can't be blank",
                "The field and message is incorrect. Expected body : can't be blank " +
                $"but received {errorResponse.Data[3].Field} : " +
                $"{errorResponse.Data[3].Message}");

            Assert.IsNull(errorResponse.Meta,
                "If the request is incorrect, the Meta data must be null, " +
                $"but received {errorResponse.Meta}");
        });
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidCommentPatchData))]
    public async Task PartiallyUpdateComment_WithInvalidData_ReturnsUnprocessableEntity(PatchCommentTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services
            .Posts().CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        // Act
        var patchRequest = new CommentPatchRequest();
        testData.PatchAction(patchRequest);

        var updatedComment = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch($"{Endpoints.Comments}/{commentId}", patchRequest);

        var errorResponse = await updatedComment
            .ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedComment.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} " +
                $" but received {updatedComment.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Message == "can't be blank, is invalid",
                "The message is incorrect. Expected can't be blank, is invalid " +
                $"but received {errorResponse.Data[0].Message}");

            Assert.IsNull(errorResponse.Meta,
                "If the request is incorrect, the Meta data must be null, " +
                $"but received {errorResponse.Meta}");
        });
    }

    // Not a unique field

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreateComment_WithExistingData_UnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId1 = createdPost!.Data.Id;

        var createdComment1 = await Services.Posts()
            .CreateCommentToPostAsync(postId1, testData.Comment.ForCreate);
        var commentId1 = createdComment1!.Data.Id;

        // Act
        var createdComment2 = await Services.Posts()
            .CreateCommentToPostAsync(postId1, testData.Comment.ForCreate);
        var commentId2 = createdComment2!.Data.Id;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsTrue(commentId2 != 0,
                "Actual ID 0. The comment was not created.");

            Assert.That(commentId2,
                Is.Not.EqualTo(commentId1),
                $"{commentId2} was expected not to be equal to {commentId1}");

            Assert.That(createdComment2.Data.Body,
                Is.EqualTo(createdComment1.Data.Body),
                $"{createdComment2.Data.Body} was expected to be equal to {createdComment2.Data.Body}");

            Assert.That(createdComment2.Data.Email,
                Is.EqualTo(createdComment1.Data.Email),
                $"{createdComment2.Data.Email} was expected to be equal to {createdComment2.Data.Email}");

            Assert.That(createdComment2.Data.PostId,
                Is.EqualTo(createdComment1.Data.PostId),
                $"{createdComment2.Data.PostId} was expected to be equal to {createdComment2.Data.PostId}");

            Assert.That(createdComment2.Data.Name,
                Is.EqualTo(createdComment1.Data.Name),
                $"{createdComment2.Data.Name} was expected to be equal to {createdComment2.Data.Name}");
        });
    }
}
