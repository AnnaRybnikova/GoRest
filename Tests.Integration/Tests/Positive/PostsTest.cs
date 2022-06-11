using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Common;
using Test.Common.Domain.Data;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.TestData;
using Test.Common.Utils;

namespace Tests.Integration.Tests.Positive;

public class PostsTest : TestBase
{
    // Positive Test Case

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreatePost(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        testData.Post.ForCreate.UserId = userId;

        // Act
        var createdPost = await Services.Posts().CreatePostAsync(testData.Post.ForCreate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(createdPost!.Data.Id,
                "Created post Id is null.");

            Assert.That(createdPost.Data.UserId,
                Is.EqualTo(userId),
                $"The received UserId {createdPost.Data.UserId} does not match the one sent " +
                $"{userId}");

            Assert.That(createdPost.Data.Title,
                Is.EqualTo(testData.Post.ForCreate.Title),
                $"The received Title {createdPost.Data.Title} does not match the one sent " +
                $"{testData.Post.ForCreate.Title}.");

            Assert.That(createdPost.Data.Body,
                Is.EqualTo(testData.Post.ForCreate.Body),
                $"The received Body {createdPost.Data.Body} does not match the one sent " +
                $"{testData.Post.ForCreate.Body}.");
        });
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task GetPost_ById(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var receivedPost = await Services.Posts().GetPostAsync(postId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(receivedPost!.Data.UserId,
                Is.EqualTo(createdPost.Data.UserId),
                $"The received UserId {receivedPost.Data.UserId} does not match the one sent " +
                $"{createdPost.Data.UserId}");

            Assert.That(receivedPost.Data.Body,
                Is.EqualTo(createdPost.Data.Body),
                $"The received Body {receivedPost.Data.Body} does not match the one sent " +
                $"{createdPost.Data.Body}");

            Assert.That(receivedPost.Data.Title,
                Is.EqualTo(createdPost.Data.Title),
                $"The received Title {receivedPost.Data.Title} does not match the one sent " +
                $"{createdPost.Data.Title}");
        });
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidDataWithQueryParamsForPost))]
    public async Task GetPosts_ByParam_ReturnOneExactPost(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var firstCreatedPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var firstPostId = firstCreatedPost!.Data.Id;

        _ = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForUpdate);

        // Act
        var postsByRequirements = await TestServices.RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Posts}?{testData.QueryParameter}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Post>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(postsByRequirements!.Data.Length,
                Is.EqualTo(1),
                $"The received Data Length {postsByRequirements.Data.Length} does not equal 1, " +
                $"so getting Post by {testData.QueryParameter.Split("=")[0]} failed");

            Assert.That(postsByRequirements.Data[0].Id,
                Is.EqualTo(firstPostId),
                $"The received Id of the first Post {postsByRequirements.Data[0].Id} does not match the one sent " +
                $"{firstPostId}");

            Assert.That(postsByRequirements.Data[0].UserId,
                Is.EqualTo(userId),
                $"The received UserId {postsByRequirements.Data[0].UserId} does not match the one sent " +
                $"{userId}");

            Assert.That(postsByRequirements.Data[0].Title,
                Is.EqualTo(testData.Post.ForCreate.Title),
                $"The received Title {postsByRequirements.Data[0].Title} does not match the one sent " +
                $"{testData.Post.ForCreate.Title}");

            Assert.That(postsByRequirements.Data[0].Body,
                Is.EqualTo(testData.Post.ForCreate.Body),
                $"The received Body {postsByRequirements.Data[0].Body} does not match the one sent " +
                $"{testData.Post.ForCreate.Body}");
        });
    }

    [Category("Get")]
    [Test]
    public async Task GetPosts()
    {
        // Act 
        var allPosts = await Services.Posts().GetPostsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(allPosts,
                "There is no posts to receive.");

            Assert.IsTrue(allPosts!.Meta.Pagination.Page == 1,
                $"The number of Pages {allPosts.Meta.Pagination.Page} is not equal to 1");
        });
    }

    [Category("Get")]
    [Test]
    public async Task GetPosts_FromPage()
    {
        // Arrange
        var allPosts = await Services.Posts().GetPostsAsync();
        var pageNumber = new Random().Next(1, allPosts!.Meta.Pagination.Pages);

        // Act
        var postsFromPage = await Services.Posts().GetPostsFromPageAsync(pageNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(postsFromPage,
                $"There is no users to receive from page {pageNumber}");

            Assert.That(postsFromPage!.Meta.Pagination.Page,
                Is.EqualTo(pageNumber),
                $"The received Page {postsFromPage.Meta.Pagination.Page} does not match the requested {pageNumber}.");

            Assert.That(postsFromPage.Meta.Pagination.Pages,
                Is.EqualTo(allPosts.Meta.Pagination.Pages),
                $"The number of Pages {postsFromPage.Meta.Pagination.Page} is incorrect, " +
                $"should be {allPosts.Meta.Pagination.Pages}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdatePost(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        testData.Post.ForUpdate.Id = createdPost.Data.Id;
        testData.Post.ForUpdate.UserId = createdPost.Data.UserId;

        // Act
        var updatedPost = await Services.Posts().UpdatePostAsync(postId, testData.Post.ForUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedPost!.Data.Id,
                Is.EqualTo(postId),
                $"The received Id {updatedPost.Data.Id} does not match the " +
                $" {postId}");

            Assert.That(updatedPost.Data.UserId,
                Is.EqualTo(userId),
                $"The received UserId {updatedPost.Data.UserId} does not match the " +
                $"{testData.Post.ForUpdate.UserId} sent to update");

            Assert.That(updatedPost.Data.Title,
                Is.EqualTo(testData.Post.ForUpdate.Title),
                $"The received Title {updatedPost.Data.Title} does not match the " +
                $"{testData.Post.ForUpdate.Title} sent to update");

            Assert.That(updatedPost.Data.Body,
                Is.EqualTo(testData.Post.ForUpdate.Body),
                $"The received Body {updatedPost.Data.Body} does not match the " +
                $"{testData.Post.ForUpdate.Body} sent to update");
        });
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.PostPatchData))]
    public async Task PartiallyUpdatePost(PatchPostTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        // Act
        var updatedPost = await Services.Posts()
            .PartialUpdatePostAsync(postId, testData.PatchAction);

        // Assert
        Assert.True(testData.PatchAssertion(updatedPost!.Data),
            "The received data does not updated.");
    }

    [Category("Delete")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task DeletePost(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForUpdate);
        var postId = createdPost!.Data.Id;

        // Act
        var deletedPostResponse = await Services.Posts().DeletePostAsync(postId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(HttpStatusCode.NoContent,
                Is.EqualTo(deletedPostResponse.HttpResponseMessage.StatusCode),
                $"The received StatusCode {deletedPostResponse.HttpResponseMessage.StatusCode} does not match" +
                $" the expected {HttpStatusCode.NoContent}.");

            Assert.IsEmpty(deletedPostResponse.HttpResponseMessage.Content.ReadAsStringAsync().Result,
                "Removal failed.");
        });
    }
}
