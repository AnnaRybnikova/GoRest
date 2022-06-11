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

public class NegativePostTest : TestBase
{
    // Without Authorization

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreatePost_Unauthorized(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var createPost = await TestServices
           .UnauthorizedRestClientFactory
           .SendRequestTo(HttpApisNames.ApiNames)
           .Post($"{Endpoints.Users}/{userId}/posts", testData.Post.ForCreate);

        // Assert
        Assert.That(createPost.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.Unauthorized),
            $"The received StatusCode {createPost.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.Unauthorized}.");
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task GetPost_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var receivedPost = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Posts + "/" + postId);

        // Assert
        Assert.That(receivedPost.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {receivedPost.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Delete")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task DeletePost_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var deletePost = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Delete(Endpoints.Posts + "/" + postId);

        // Assert
        Assert.That(deletePost.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {deletePost.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.PostPatchData))]
    public async Task PartiallyUpdatePost_Unauthorized_ReturnsNotFound(PatchPostTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var patchRequest = new PostPatchRequest();
        testData.PatchAction(patchRequest);

        var updatedPost = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch(Endpoints.Posts + "/" + postId, patchRequest);

        // Assert
        Assert.That(updatedPost.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {updatedPost.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdatePost_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var updatedPost = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put(Endpoints.Posts + "/" + postId, testData.Post.ForUpdate);

        // Assert
        Assert.That(updatedPost.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {updatedPost.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    // Incorrect Data

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidData))]
    public async Task CreatePost_WithInvalidData_ReturnsUnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var createPost = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post($"{Endpoints.Users}/{userId}/posts", testData.InvalidPost.ForCreate);

        var errorResponse = await createPost.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createPost.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} but received " +
                $"{createPost.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Field == "title"
                          && errorResponse.Data[0].Message == "can't be blank",
                "The field and message is incorrect. Expected title : can't be blank " +
                $"but received {errorResponse.Data[0].Field} : " +
                $"{errorResponse.Data[0].Message}");

            Assert.IsTrue(errorResponse.Data[1].Field == "body"
                          && errorResponse.Data[1].Message == "can't be blank",
                "The field and message is incorrect. Expected body : can't be blank " +
                $"but received {errorResponse.Data[1].Field} : " +
                $"{errorResponse.Data[1].Message}");

            Assert.IsNull(errorResponse.Meta,
                "If the request is incorrect, the Meta data must be null, " +
                $"but received {errorResponse.Meta}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidData))]
    public async Task UpdatePost_WithInvalidData_ReturnsUnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var updatedPost = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Posts}/{postId}", testData.InvalidPost.ForUpdate);

        var errorResponse = await updatedPost.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedPost.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} but received " +
                $"{updatedPost.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Field == "user"
                          && errorResponse.Data[0].Message == "must exist",
                "The field and message is incorrect. Expected user : must exist " +
                $"but received {errorResponse.Data[0].Field} : " +
                $"{errorResponse.Data[0].Message}");

            Assert.IsTrue(errorResponse.Data[1].Field == "title"
                          && errorResponse.Data[1].Message == "can't be blank",
                "The field and message is incorrect. Expected title : can't be blank " +
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

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidPostPatchData))]
    public async Task PartiallyUpdatePost_WithInvalidData_ReturnsUnprocessableEntity(PatchPostTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var patchRequest = new PostPatchRequest();
        testData.PatchAction(patchRequest);

        var updatedPost = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch($"{Endpoints.Posts}/{postId}", patchRequest);

        var errorResponse = await updatedPost.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedPost.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity}" +
                $" but received {updatedPost.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Message == "can't be blank",
                "The message is incorrect. Expected can't be blank " +
                $"but received {errorResponse.Data[0].Message}");

            Assert.IsNull(errorResponse.Meta,
                "If the request is incorrect, the Meta data must be null, " +
                $"but received {errorResponse.Meta}");
        });
    }

    // Not a unique field

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreatePost_WithExistingData_UnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost1 = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId1 = createdPost1!.Data.Id;

        testData.Post.ForCreate.UserId = userId;

        // Act
        var createdPost2 = await Services.Posts().CreatePostAsync(testData.Post.ForCreate);
        var postId2 = createdPost2!.Data.Id;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsTrue(postId2 != 0,
                "Actual ID 0. The comment was not created.");

            Assert.That(postId2,
                Is.Not.EqualTo(postId1),
                $"{postId2} was expected to not be equal to { postId1}");

            Assert.That(createdPost2.Data.Body,
                Is.EqualTo(createdPost1.Data.Body),
                $"{createdPost2.Data.Body} was expected to be equal to {createdPost1.Data.Body}");

            Assert.That(createdPost2.Data.Title,
                Is.EqualTo(createdPost1.Data.Title),
                $"{createdPost2.Data.Title} was expected to be equal to {createdPost1.Data.Title}");

            Assert.That(createdPost2.Data.UserId,
                Is.EqualTo(createdPost1.Data.UserId),
                $"{createdPost2.Data.UserId} was expected to be equal to {createdPost1.Data.UserId}");
        });
    }
}
