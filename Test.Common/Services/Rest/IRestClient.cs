using System.Threading.Tasks;

namespace Test.Common.Services.Rest;

public interface IRestClient
{
    public Task<RestResponse> Post<T>(string endpoint, T payload) where T : class;

    public Task<RestResponse> Put<T>(string endpoint, T payload) where T : class;

    public Task<RestResponse> Patch<T>(string endpoint, T payload) where T : class;

    public Task<RestResponse> Get(string endpoint);

    public Task<RestResponse> Delete(string endpoint);
}
