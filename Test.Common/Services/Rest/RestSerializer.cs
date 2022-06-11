using System;
using Newtonsoft.Json;
using Test.Common.Options.Enums;
using Test.Common.Utils;

namespace Test.Common.Services.Rest;

public class RestSerializer : IRestSerializer
{
    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = JsonHelper.CamelCaseContractResolver,
        Formatting = Formatting.Indented
    };

    private readonly RestEncoding _encoding;

    public RestSerializer(RestEncoding encoding)
    {
        _encoding = encoding;
    }

    public string Serialize<T>(T data) where T : class
    {
        return _encoding switch
        {
            RestEncoding.Default or RestEncoding.Json => JsonConvert.SerializeObject(data, _jsonSettings),
            RestEncoding.Xml => XmlConvert.SerializeObject(data),
            _ => throw new InvalidOperationException("Can not serialize object with unknown encoding.")
        };
    }

    public T? Deserialize<T>(string str) where T : class
    {
        return _encoding switch
        {
            RestEncoding.Default or RestEncoding.Json => JsonConvert.DeserializeObject<T>(str),
            RestEncoding.Xml => XmlConvert.DeserializeObject<T>(str),
            _ => throw new InvalidOperationException("Can not serialize object with unknown encoding.")
        };
    }
}
