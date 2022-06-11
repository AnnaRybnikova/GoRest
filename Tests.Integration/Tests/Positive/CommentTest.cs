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

public class CommentTest : TestBase
{
    // Positive Test Case

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreateComment(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        testData.Comment.ForCreate.PostId = postId;

        // Act
        var createdComment = await Services.Comments().CreateCommentAsync(testData.Comment.ForCreate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(createdComment!.Data.Id,
                "Created comment Id is null.");

            Assert.That(createdComment.Data.Email,
                Is.EqualTo(testData.Comment.ForCreate.Email),
                $"The received Email {createdComment.Data.Email} does not match the one sent " +
                $"{testData.Comment.ForCreate.Email}.");

            Assert.That(createdComment.Data.Body,
                Is.EqualTo(testData.Comment.ForCreate.Body),
                $"The received Body {createdComment.Data.Body} does not match the one sent " +
                $"{testData.Comment.ForCreate.Body}.");

            Assert.That(createdComment.Data.PostId,
                Is.EqualTo(testData.Comment.ForCreate.PostId),
                $"The received PostId {createdComment.Data.PostId} does not match the one sent " +
                $"{testData.Comment.ForCreate.PostId}.");

            Assert.That(createdComment.Data.Name,
                Is.EqualTo(testData.Comment.ForCreate.Name),
                $"The received Name {createdComment.Data.Name} does not match the one sent " +
                $"{testData.Comment.ForCreate.Name}.");
        });
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task GetComment_ById(TestData testData)
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
        var receivedComment = await Services.Comments().GetCommentAsync(commentId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(receivedComment!.Data.PostId,
                Is.EqualTo(createdComment.Data.PostId),
                $"The received PostId {receivedComment.Data.PostId} does not match the one sent " +
                $"{createdComment.Data.PostId}.");

            Assert.That(receivedComment.Data.Name,
                Is.EqualTo(createdComment.Data.Name),
                $"The received Name {receivedComment.Data.Name} does not match the one sent " +
                $"{createdComment.Data.Name}.");

            Assert.That(receivedComment.Data.Body,
                Is.EqualTo(createdComment.Data.Body),
                $"The received Body {receivedComment.Data.Body} does not match the one sent " +
                $"{createdComment.Data.Body}.");

            Assert.That(receivedComment.Data.Email,
                Is.EqualTo(createdComment.Data.Email),
                $"The received Email {receivedComment.Data.Email} does not match the one sent " +
                $"{createdComment.Data.Email}, or the given email is incorrect.");
        });
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidDataWithQueryParamsForComments))]
    public async Task GetComments_ByParam_ReturnOneExactComment(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdFirstComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var firstCommentId = createdFirstComment!.Data.Id;

        await Services.Posts().CreateCommentToPostAsync(postId, testData.Comment.ForUpdate);

        // Act
        var commentsByRequirements = await TestServices.RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Comments}?{testData.QueryParameter}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<Comment>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(commentsByRequirements!.Data.Length,
                Is.EqualTo(1),
                $"The received Data Length {commentsByRequirements.Data.Length} does not equal 1, " +
                $"so getting Comment by {testData.QueryParameter.Split("=")[0]} failed");

            Assert.That(commentsByRequirements.Data[0].Id,
                Is.EqualTo(firstCommentId),
                $"The received Id of the first Comment {commentsByRequirements.Data[0].Id} does not match the one sent " +
                $"{firstCommentId}");

            Assert.That(commentsByRequirements.Data[0].PostId,
                Is.EqualTo(createdFirstComment.Data.PostId),
                $"The received PostId {commentsByRequirements.Data[0].PostId} does not match the one sent " +
                $"{createdFirstComment.Data.PostId}.");

            Assert.That(commentsByRequirements.Data[0].Name,
                Is.EqualTo(createdFirstComment.Data.Name),
                $"The received Name {commentsByRequirements.Data[0].Name} does not match the one sent " +
                $"{createdFirstComment.Data.Name}.");

            Assert.That(commentsByRequirements.Data[0].Email,
                Is.EqualTo(createdFirstComment.Data.Email),
                $"The received Email {commentsByRequirements.Data[0].Email} does not match the one sent " +
                $"{createdFirstComment.Data.Email}, or the given email is incorrect.");

            Assert.That(commentsByRequirements.Data[0].Body,
                Is.EqualTo(createdFirstComment.Data.Body),
                $"The received Body {commentsByRequirements.Data[0].Body} does not match the one sent " +
                $"{createdFirstComment.Data.Body}.");
        });
    }

    [Category("Get")]
    [Test]
    public async Task GetComments()
    {
        // Act 
        var allComments = await Services.Comments().GetCommentsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(allComments,
                "There is no comments to receive.");

            Assert.That(allComments!.Meta.Pagination.Page,
                Is.EqualTo(1),
                $"The number of Pages {allComments.Meta.Pagination.Page} is not equal to 1");
        });
    }

    [Category("Get")]
    [Test]
    public async Task GetComments_FromPage()
    {
        // Arrange
        var allComments = await Services.Comments().GetCommentsAsync();
        var pageNumber = new Random().Next(1, allComments!.Meta.Pagination.Pages);

        // Act
        var commentsFromPage = await Services.Comments().GetCommentsFromPageAsync(pageNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(commentsFromPage,
                $"There is no users to receive from page {pageNumber}");

            Assert.That(commentsFromPage!.Meta.Pagination.Page,
                Is.EqualTo(pageNumber),
                $"The received Page {commentsFromPage.Meta.Pagination.Page} does not match the requested {pageNumber}");

            Assert.That(commentsFromPage.Meta.Pagination.Pages,
                Is.EqualTo(allComments.Meta.Pagination.Pages),
                $"The number of Pages {commentsFromPage.Meta.Pagination.Page} is incorrect, " +
                $"should be {allComments.Meta.Pagination.Pages}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdateComment(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        var createdPost = await Services.Users().CreatePostToUserAsync(userId, testData.Post.ForCreate);
        var postId = createdPost!.Data.Id;

        var createdComment = await Services.Posts()
            .CreateCommentToPostAsync(postId, testData.Comment.ForCreate);
        var commentId = createdComment!.Data.Id;

        testData.Comment.ForUpdate.PostId = createdComment.Data.PostId;
        testData.Comment.ForUpdate.Id = createdComment.Data.Id;

        // Act
        var updatedComment = await Services.Comments()
            .UpdateCommentAsync(commentId, testData.Comment.ForUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedComment!.Data.Id,
                Is.EqualTo(commentId),
                $"The received Id {updatedComment.Data.Id} does not match the one sent " +
                $"{commentId}");

            Assert.That(updatedComment.Data.PostId,
                Is.EqualTo(testData.Comment.ForUpdate.PostId),
                $"The received PostId {updatedComment.Data.PostId} does not match the ome sent " +
                $"{testData.Comment.ForUpdate.PostId}.");

            Assert.That(updatedComment.Data.Name,
                Is.EqualTo(testData.Comment.ForUpdate.Name),
                $"The received Name {updatedComment.Data.Name} does not match the one sent " +
                $"{testData.Comment.ForUpdate.Name}.");

            Assert.That(updatedComment.Data.Email,
                Is.EqualTo(testData.Comment.ForUpdate.Email),
                $"The received Email {updatedComment.Data.Email} does not match the one sent " +
                $"{testData.Comment.ForUpdate.Email}.");

            Assert.That(updatedComment.Data.Body,
                Is.EqualTo(testData.Comment.ForUpdate.Body),
                $"The received Body {updatedComment.Data.Body} does not match the one sent " +
                $"{testData.Comment.ForUpdate.Body}.");
        });
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.CommentPatchData))]
    public async Task PartiallyUpdateComment(PatchCommentTestData testData)
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
        var updatedComment = await Services.Comments()
            .PartialUpdateCommentAsync(commentId, testData.PatchAction);

        //Assert
        Assert.True(testData.PatchAssertion(updatedComment!.Data),
            "The received data does not updated.");
    }

    [Category("Delete")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task DeleteComment(TestData testData)
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
        var deletedCommentResponse = await Services.Comments().DeleteCommentAsync(commentId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(HttpStatusCode.NoContent,
                Is.EqualTo(deletedCommentResponse.HttpResponseMessage.StatusCode),
                $"The received StatusCode {deletedCommentResponse.HttpResponseMessage.StatusCode} does not match" +
                $"the expected {HttpStatusCode.NoContent}.");

            Assert.IsEmpty(deletedCommentResponse.HttpResponseMessage.Content.ReadAsStringAsync().Result,
                "Removal failed.");
        });
    }
}
