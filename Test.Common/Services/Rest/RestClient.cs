using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Test.Common.Options.Enums;

namespace Test.Common.Services.Rest;

public class RestClient : IRestClient
{
    private readonly HttpClient _httpClient;

    private readonly IRestSerializer _serializer;

    private readonly string _mediaType;

    private readonly string _pathPostfix;

    public RestClient(HttpClient httpClient, RestEncoding encoding)
    {
        _httpClient = httpClient;

        (_mediaType, _pathPostfix) = encoding switch
        {
            RestEncoding.Default => ("application/json", ""),
            RestEncoding.Json => ("application/json", ".json"),
            RestEncoding.Xml => ("application/xml", ".xml"),
            _ => throw new ArgumentException($"Unsupported rest encoding: {encoding}.")
        };

        _serializer = new RestSerializer(encoding);
    }

    public async Task<RestResponse> Post<T>(string endpoint, T payload) where T : class
    {
        var httpResponse = await _httpClient
            .PostAsync($"{endpoint}{_pathPostfix}",
                new StringContent(_serializer.Serialize(payload), Encoding.UTF8, _mediaType));

        return new RestResponse(httpResponse, _serializer);
    }

    public async Task<RestResponse> Put<T>(string endpoint, T payload) where T : class
    {
        var httpResponse = await _httpClient
            .PutAsync($"{endpoint}{_pathPostfix}",
                new StringContent(_serializer.Serialize(payload), Encoding.UTF8, _mediaType));

        return new RestResponse(httpResponse, _serializer);
    }

    public async Task<RestResponse> Patch<T>(string endpoint, T payload) where T : class
    {
        var httpResponse = await _httpClient
            .PatchAsync($"{endpoint}{_pathPostfix}",
                new StringContent(_serializer.Serialize(payload), Encoding.UTF8, _mediaType));

        return new RestResponse(httpResponse, _serializer);
    }

    public async Task<RestResponse> Get(string endpoint)
    {
        var httpResponse = await _httpClient
            .GetAsync($"{endpoint}{_pathPostfix}");
        return new RestResponse(httpResponse, _serializer);
    }

    public async Task<RestResponse> Delete(string endpoint)
    {
        var httpResponse = await _httpClient
            .DeleteAsync($"{endpoint}{_pathPostfix}");
        return new RestResponse(httpResponse, _serializer);
    }
}
