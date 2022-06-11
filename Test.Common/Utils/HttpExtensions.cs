using System.Threading.Tasks;
using Test.Common.Services.Rest;

namespace Test.Common.Utils;

public static class HttpExtensions
{
    public static async Task<T?> ReadAsync<T>(this Task<RestResponse> getRestResponse) where T : class
    {
        var restResponse = await getRestResponse;
        return await restResponse.ReadResponseAsync<T>();
    }

    public static async Task<RestResponse> EnsureSuccessStatusCodeAsync(
        this Task<RestResponse> getRestResponseAsync)
    {
        var restResponse = await getRestResponseAsync;
        _ = restResponse.HttpResponseMessage.EnsureSuccessStatusCode();
        return restResponse;
    }
}
