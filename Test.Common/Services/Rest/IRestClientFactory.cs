namespace Test.Common.Services.Rest;

public interface IRestClientFactory
{
    IRestClientFactory Unauthorized();

    IRestClient SendRequestTo(string name);
}
