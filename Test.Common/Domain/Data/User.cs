using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Test.Common.Domain.Data.Enums;

namespace Test.Common.Domain.Data;

[XmlRoot("user")]
public class User
{
    [XmlElement("id")]
    public int Id { get; set; }

    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("gender")]
    [JsonConverter(typeof(StringEnumConverter))]
    public Gender? Gender { get; set; }

    [XmlElement("status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public Status? Status { get; set; }

    [XmlElement("email")]
    public string? Email { get; set; }
}
