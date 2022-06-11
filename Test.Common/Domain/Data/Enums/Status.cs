using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Test.Common.Domain.Data.Enums;

public enum Status
{
    [XmlEnum("active")]
    [JsonProperty("active")]
    Active,

    [XmlEnum("inactive")]
    [JsonProperty("inactive")]
    Inactive
}
