using Newtonsoft.Json.Serialization;

namespace Test.Common.Utils;

internal static class JsonHelper
{
    public static readonly DefaultContractResolver CamelCaseContractResolver = new()
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };
}
