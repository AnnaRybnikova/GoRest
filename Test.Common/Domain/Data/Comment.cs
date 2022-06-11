using Newtonsoft.Json;

namespace Test.Common.Domain.Data;

public class Comment
{
    public int Id { get; set; }
    
    [JsonProperty("post_id")]
    public int PostId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Body { get; set; }
}
