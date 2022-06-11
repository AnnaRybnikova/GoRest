using System.Net.Http;
using System.Threading.Tasks;

namespace Test.Common.Services.Rest;

public class RestResponse
{
    private readonly IRestSerializer _serializer;

    public HttpResponseMessage HttpResponseMessage { get; }

    public RestResponse(HttpResponseMessage httpResponseMessage, IRestSerializer serializer)
    {
        _serializer = serializer;
        HttpResponseMessage = httpResponseMessage;
    }

    public async Task<T?> ReadResponseAsync<T>() where T : class
    {
        return _serializer.Deserialize<T>(await HttpResponseMessage.Content.ReadAsStringAsync());
    }
}
