using Newtonsoft.Json;

namespace Test.Common.Domain.Data;

public class Post
{
    public int Id { get; set; }

    [JsonProperty("user_id")]
    public int UserId { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }
}
