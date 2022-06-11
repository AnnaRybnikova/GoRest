using Newtonsoft.Json;

namespace Test.Common.Domain.Data.Requests;

public class CommentPatchRequest
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? PostId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Email { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Body { get; set; }
}
