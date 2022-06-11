using Newtonsoft.Json;

namespace Test.Common.Domain.Data.Requests;

public class PostPatchRequest
{
    [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? UserId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Title { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Body { get; set; }
}
