using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Test.Common.Options;
using Test.Common.Options.Enums;

namespace Test.Common.Services.Rest;

public class RestClientFactory : IRestClientFactory
{
    private readonly IOptions<TestServicesOptions> _options;

    private readonly ConcurrentDictionary<string, HttpClient> _httpApis = new();

    private readonly RestEncoding _encoding;

    public RestClientFactory(IOptions<TestServicesOptions> options) : this(options, true)
    {
    }
    private RestClientFactory(IOptions<TestServicesOptions> options, bool authorized = true)
    {
        _options = options;
        var client = new HttpClient
        {
            BaseAddress = new Uri(options.Value.HttpLinksOptions.GoRest)
        };

        if (authorized)
        {
            client.DefaultRequestHeaders
                .Add("Authorization", $"Bearer {options.Value.HttpLinksOptions.GoRestToken}");

            _ = client.DefaultRequestHeaders.UserAgent
                .TryParseAdd("C# Application test");
        }
        _httpApis[HttpApisNames.ApiNames] = client;
        _encoding = options.Value.HttpLinksOptions.Encoding;
    }

    public IRestClientFactory Unauthorized()
    {
        return new RestClientFactory(_options, false);
    }

    public IRestClient SendRequestTo(string name) => new RestClient(_httpApis[name], _encoding);
}
