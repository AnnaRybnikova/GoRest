using System.Net;
using NUnit.Framework;
using System.Threading.Tasks;
using Test.Common;
using Test.Common.Domain.Data.Requests;
using Test.Common.Domain.Data.Responses;
using Test.Common.Domain.ResponseData;
using Test.Common.Services.Rest;
using Test.Common.TestData;

namespace Tests.Integration.Tests.Negative;

public class NegativeUserTest : TestBase
{
    // Without Authorization

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreateUser_Unauthorized(TestData testData)
    {
        // Act
        var createdUser = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post(Endpoints.Users, testData.User.ForCreate);

        // Assert
        Assert.That(createdUser.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.Unauthorized),
            $"The received StatusCode {createdUser.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.Unauthorized}.");
    }

    [Category("Get")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task GetUser_Unauthorized(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var receivedUser = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Get(Endpoints.Users + "/" + userId);

        // Assert
        Assert.That(receivedUser.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {receivedUser.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Delete")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task DeleteUser_Unauthorized(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var deletedUser = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Delete(Endpoints.Users + "/" + userId);

        // Assert
        Assert.That(deletedUser.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {deletedUser.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.UserPatchData))]
    public async Task PartiallyUpdateUser_Unauthorized_ReturnsNotFound(PatchUserTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var patchRequest = new UserPatchRequest();
        testData.PatchAction(patchRequest);

        var updatedUser = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch(Endpoints.Users + "/" + userId, patchRequest);

        // Assert
        Assert.That(updatedUser.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {updatedUser.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdateUser_Unauthorized_ReturnsNotFound(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var updatedUser = await TestServices
            .UnauthorizedRestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put(Endpoints.Users + "/" + userId, testData.User.ForUpdate);

        // Assert
        Assert.That(updatedUser.HttpResponseMessage.StatusCode,
            Is.EqualTo(HttpStatusCode.NotFound),
            $"The received StatusCode {updatedUser.HttpResponseMessage.StatusCode} does not match " +
            $"the expected {HttpStatusCode.NotFound}.");
    }

    // Incorrect Data

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidData))]
    public async Task CreateUser_WithInvalidData_ReturnsUnprocessableEntity(TestData testData)
    {
        // Act
        var createdUser = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post(Endpoints.Users, testData.InvalidUser.ForCreate);

        var errorResponse = await createdUser.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createdUser.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} but received " +
                $"{createdUser.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Field == "email"
                          && errorResponse.Data[0].Message == "can't be blank",
                "The field and message is incorrect. Expected email : can't be blank " +
                $"but received {errorResponse.Data[0].Field} : " +
                $"{errorResponse.Data[0].Message}");

            Assert.IsTrue(errorResponse.Data[1].Field == "name"
                          && errorResponse.Data[1].Message == "can't be blank",
                "The field and message is incorrect. Expected name : can't be blank " +
                $"but received {errorResponse.Data[1].Field} : " +
                $"{errorResponse.Data[1].Message}");

            Assert.IsTrue(errorResponse.Data[2].Field == "gender"
                          && errorResponse.Data[2].Message == "can't be blank, can be male or female",
                "The field and message is incorrect. Expected gender : can't be blank, can be male or female " +
                $"but received {errorResponse.Data[2].Field} : " +
                $"{errorResponse.Data[2].Message}");

            Assert.IsTrue(errorResponse.Data[3].Field == "status"
                          && errorResponse.Data[3].Message == "can't be blank",
                "The field and message is incorrect. Expected status : can't be blank " +
                $"but received {errorResponse.Data[3].Field} : " +
                $"{errorResponse.Data[3].Message}");

            Assert.IsNull(errorResponse.Meta,
                $"Meta is incorrect. Expected {null} but received {errorResponse.Meta}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidData))]
    public async Task UpdateUser_WithInvalidData_ReturnsUnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var updatedUser = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Users}/{userId}", testData.InvalidUser.ForUpdate);

        var errorResponse = await updatedUser.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedUser.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} " +
                $" but received {updatedUser.HttpResponseMessage.StatusCode}");

            Assert.IsTrue(errorResponse!.Data[0].Field == "email"
                          && errorResponse.Data[0].Message == "can't be blank",
                "The field and message is incorrect. Expected email : can't be blank " +
                $"but received {errorResponse.Data[0].Field} : " +
                $"{errorResponse.Data[0].Message}");

            Assert.IsTrue(errorResponse.Data[1].Field == "name"
                          && errorResponse.Data[1].Message == "can't be blank",
                "The field and message is incorrect. Expected name : can't be blank " +
                $"but received {errorResponse.Data[1].Field} : " +
                $"{errorResponse.Data[1].Message}");

            Assert.IsTrue(errorResponse.Data[2].Field == "gender"
                          && errorResponse.Data[2].Message == "can't be blank, can be male or female",
                "The field and message is incorrect. Expected gender : can't be blank, can be male or female " +
                $"but received {errorResponse.Data[2].Field} : " +
                $"{errorResponse.Data[2].Message}");

            Assert.IsTrue(errorResponse.Data[3].Field == "status"
                          && errorResponse.Data[3].Message == "can't be blank",
                "The field and message is incorrect. Expected status : can't be blank " +
                $"but received {errorResponse.Data[3].Field} : " +
                $"{errorResponse.Data[3].Message}");

            Assert.IsNull(errorResponse.Meta,
                $"Meta is incorrect. Expected {null} " +
                $"but received {errorResponse.Meta}");
        });
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.InvalidUserPatchData))]
    public async Task PartiallyUpdateUser_WithInvalidData_ReturnsUnprocessableEntity(PatchUserTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        // Act
        var patchRequest = new UserPatchRequest();
        testData.PatchAction(patchRequest);

        var updatedUser = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch($"{Endpoints.Users}/{userId}", patchRequest);

        var errorResponse = await updatedUser.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedUser.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code is incorrect. Expected {HttpStatusCode.UnprocessableEntity} " +
                $"but received {updatedUser.HttpResponseMessage.StatusCode}");

            Assert.That(errorResponse!.Data[0].Message,
                Is.EqualTo("can't be blank"),
                "The message is incorrect. Expected can't be blank " +
                $"but received {errorResponse.Data[0].Message}");

            Assert.IsNull(errorResponse.Meta,
                $"Meta is incorrect. Expected {null} but received {errorResponse.Meta}");
        });
    }

    // Not a unique field

    [Category("Post")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task CreateUser_WithExistingData_UnprocessableEntity(TestData testData)
    {
        // Arrange
        _ = await Services.Users().CreateUserAsync(testData.User.ForCreate);

        // Act
        var createdUser = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Post(Endpoints.Users, testData.User.ForCreate);

        var errorResponse = await createdUser.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createdUser.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code should be {HttpStatusCode.UnprocessableEntity}");

            Assert.IsNull(errorResponse!.Meta,
                $"Meta is incorrect. Expected {null} but received {errorResponse.Meta}");

            Assert.That(errorResponse.Data[0].Message,
                Is.EqualTo("has already been taken"),
                $"Expected has already been taken but received {errorResponse.Data[0].Message}");
        });
    }

    [Category("Patch")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ExistingUserEmailPatchData))]
    public async Task PartiallyUpdateUser_WithExistingData_UnprocessableEntity(PatchUserTestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        _ = await Services.Users().CreateUserAsync(testData.User.ForUpdate);

        // Act
        var patchRequest = new UserPatchRequest();
        testData.PatchAction(patchRequest);

        var updateUser = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Patch($"{Endpoints.Users}/{userId}", patchRequest);

        var errorResponse = await updateUser.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updateUser.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code should be {HttpStatusCode.UnprocessableEntity}");

            Assert.IsNull(errorResponse!.Meta,
                $"Meta is incorrect. Expected {null} but received {errorResponse.Meta}");

            Assert.That(errorResponse.Data[0].Field,
                Is.EqualTo("email"),
                $"Expected email but received {errorResponse.Data[0].Field}");

            Assert.That(errorResponse.Data[0].Message,
                Is.EqualTo("has already been taken"),
                $"Expected has already been taken but received {errorResponse.Data[0].Message}");
        });
    }

    [Category("Put")]
    [TestCaseSource(typeof(CreateTestData), nameof(CreateTestData.ValidData))]
    public async Task UpdateUser_WithExistingData_UnprocessableEntity(TestData testData)
    {
        // Arrange
        var createdUser = await Services.Users().CreateUserAsync(testData.User.ForCreate);
        var userId = createdUser!.Data.Id;

        _ = await Services.Users().CreateUserAsync(testData.User.ForUpdate);

        // Act
        var updatedUser = await TestServices
            .RestClientFactory
            .SendRequestTo(HttpApisNames.ApiNames)
            .Put($"{Endpoints.Users}/{userId}", testData.User.ForUpdate);

        var errorResponse = await updatedUser.ReadResponseAsync<CollectionResponse<ErrorResponse>>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedUser.HttpResponseMessage.StatusCode,
                Is.EqualTo(HttpStatusCode.UnprocessableEntity),
                $"Status Code should be {HttpStatusCode.UnprocessableEntity}");

            Assert.IsNull(errorResponse!.Meta,
                $"Meta is incorrect. Expected {null} but received {errorResponse.Meta}");

            Assert.That(errorResponse.Data[0].Field,
                Is.EqualTo("email"),
                $"Expected email but received {errorResponse.Data[0].Field}");

            Assert.That(errorResponse.Data[0].Message,
                Is.EqualTo("has already been taken"),
                $"Expected has already been taken but received {errorResponse.Data[0].Message}");
        });
    }
}
