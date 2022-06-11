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

public class UsersTest : TestBase
{
    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreateUser(TestData testData)
    {
        // Act
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(createdUser!.Data.Id,
                "Created user Id is null.");

            Assert.That(createdUser.Data.Name,
                Is.EqualTo(testData.User.ForCreate.Name),
                $"The received Name {createdUser.Data.Name} does not match the one sent " +
                $"{testData.User.ForCreate.Name}");

            Assert.That(createdUser.Data.Email,
                Is.EqualTo(testData.User.ForCreate.Email),
                $"The received Email {createdUser.Data.Email} does not match the one sent " +
                $"{testData.User.ForCreate.Email}");

            Assert.That(createdUser.Data.Gender,
                Is.EqualTo(testData.User.ForCreate.Gender),
                $"The received Gender {createdUser.Data.Gender} does not match the one sent " +
                $"{testData.User.ForCreate.Gender}");

            Assert.That(createdUser.Data.Status,
                Is.EqualTo(testData.User.ForCreate.Status),
                $"The received Status {createdUser.Data.Status} does not match the one sent " +
                $"{testData.User.ForCreate.Status}");
        });
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task GetUser_ById(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var receivedUser = await Services.Users().GetUserAsync(userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(receivedUser!.Data.Name,
                Is.EqualTo(createdUser.Data.Name),
                $"The received Name {receivedUser.Data.Name} does not match the one sent " +
                $"{createdUser.Data.Name}");

            Assert.That(receivedUser.Data.Email,
                Is.EqualTo(createdUser.Data.Email),
                $"The received Email {receivedUser.Data.Email} does not match the one sent " +
                $"{createdUser.Data.Email}");

            Assert.That(receivedUser.Data.Gender,
                Is.EqualTo(createdUser.Data.Gender),
                $"The received Gender {receivedUser.Data.Gender} does not match the one sent " +
                $"{createdUser.Data.Gender}");

            Assert.That(receivedUser.Data.Status,
                Is.EqualTo(createdUser.Data.Status),
                $"The received Status {receivedUser.Data.Status} does not match the one sent " +
                $"{createdUser.Data.Status}");
        });
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidDataWithQueryParamsForUser))]
    public async Task GetUsers_ByParam_ReturnOneExactUser(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var firstUserId = createdUser!.Data.Id;

        await Services.Users().CreateUserAsync(testData.User.ForUpdate);

        // Act
        var usersByRequirements = await TestServices.RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get($"{Endpoints.Users}?{testData.QueryParameter}")
            .EnsureSuccessStatusCodeAsync()
            .ReadAsync<CollectionResponse<User>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(usersByRequirements!.Data.Length,
                Is.EqualTo(1),
                $"The received Data Length {usersByRequirements.Data.Length} does not equal 1, " +
                $"so getting User by {testData.QueryParameter.Split("=")[0]} failed");

            Assert.That(usersByRequirements.Data[0].Id,
                Is.EqualTo(firstUserId),
                $"The received Id of first User {usersByRequirements.Data[0].Id} does not match the one sent " +
                $"{firstUserId}");

            Assert.That(usersByRequirements.Data[0].Name,
                Is.EqualTo(createdUser.Data.Name),
                $"The received Name {usersByRequirements.Data[0].Name} does not match the one sent " +
                $"{createdUser.Data.Name}");

            Assert.That(usersByRequirements.Data[0].Email,
                Is.EqualTo(createdUser.Data.Email),
                $"The received Email {usersByRequirements.Data[0].Email} does not match the one sent " +
                $"{createdUser.Data.Email}");

            Assert.That(usersByRequirements.Data[0].Gender,
                Is.EqualTo(createdUser.Data.Gender),
                $"The received Gender {usersByRequirements.Data[0].Gender} does not match the one sent " +
                $"{createdUser.Data.Gender}");

            Assert.That(usersByRequirements.Data[0].Status,
                Is.EqualTo(createdUser.Data.Status),
                $"The received Status {usersByRequirements.Data[0].Status} does not match the one sent " +
                $"{createdUser.Data.Status}");
        });
    }

    [Category("Get")]
    [Test]
    public async Task GetUsers()
    {
        // Act
        var allUsers = await Services.Users().GetUsersAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(allUsers,
                "There is no users to receive.");

            Assert.That(allUsers!.Meta.Pagination.Page,
                Is.EqualTo(1),
                $"The number of Pages {allUsers.Meta.Pagination.Page} is not equal to 1");
        });
    }

    [Category("Get")]
    [Test]
    public async Task GetUsers_FromPage()
    {
        // Arrange
        var allPosts = await Services.Users().GetUsersAsync();
        var pageNumber = new Random().Next(1, allPosts!.Meta.Pagination.Pages);

        // Act
        var usersFromPage = await Services.Users().GetUsersFromPageAsync(pageNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(usersFromPage,
                $"There is no users to receive from page {pageNumber}");

            Assert.That(usersFromPage!.Meta.Pagination.Page,
                Is.EqualTo(pageNumber),
                $"The received Page {usersFromPage.Meta.Pagination.Page} does not match the requested {pageNumber}.");

            Assert.That(usersFromPage.Meta.Pagination.Pages,
                Is.EqualTo(allPosts.Meta.Pagination.Pages),
                $"The number of Pages {usersFromPage.Meta.Pagination.Page} is incorrect, " +
                $"should be {allPosts.Meta.Pagination.Pages}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdateUser(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var updatedUser = await Services.Users().UpdateUserAsync(userId, testData.User.ForUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedUser!.Data.Id,
                Is.EqualTo(userId),
                $"The received Id {updatedUser.Data.Id} does not match the {userId}");

            Assert.That(updatedUser.Data.Name,
                Is.EqualTo(testData.User.ForUpdate.Name),
                $"The received Name {updatedUser.Data.Name} does not match the " +
                $"{testData.User.ForUpdate.Name} sent to update");

            Assert.That(updatedUser.Data.Gender,
                Is.EqualTo(testData.User.ForUpdate.Gender),
                $"The received Gender {updatedUser.Data.Gender} does not match the " +
                $"{testData.User.ForUpdate.Gender} sent to update");

            Assert.That(updatedUser.Data.Status,
                Is.EqualTo(testData.User.ForUpdate.Status),
                $"The received Status {updatedUser.Data.Status} does not match the " +
                $"{testData.User.ForUpdate.Status} sent to update");

            Assert.That(updatedUser.Data.Email,
                Is.EqualTo(testData.User.ForUpdate.Email),
                $"The received Email {updatedUser.Data.Email} does not match the " +
                $"{testData.User.ForUpdate.Email} sent to update");
        });
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.UserPatchData))]
    public async Task PartiallyUpdateUser(PatchUserTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var updatedUser = await Services.Users()
            .PartialUpdateUserAsync(userId, testData.PatchAction);

        // Assert
        Assert.True(testData.PatchAssertion(updatedUser!.Data),
            "The received data does not updated.");
    }

    [Category("Delete")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task DeleteUser(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var deletedUserResponse = await Services.Users().DeleteUserAsync(userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(HttpStatusCode.NoContent,
                Is.EqualTo(deletedUserResponse.HttpResponseMessage.StatusCode),
                $"The received StatusCode {deletedUserResponse.HttpResponseMessage.StatusCode} does not match" +
                $" the expected {HttpStatusCode.NoContent}.");

            Assert.IsEmpty(deletedUserResponse.HttpResponseMessage.Content.ReadAsStringAsync().Result,
                "Removal failed.");
        });
    }
}
