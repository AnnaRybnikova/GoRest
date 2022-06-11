using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Test.Common.Domain.Data.Enums;

namespace Test.Common.Domain.Data.Requests;

public class UserPatchRequest
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(StringEnumConverter))]
    public Gender? Gender { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(StringEnumConverter))]
    public Status? Status { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }
}
