using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Test.Common.Domain.Data.Enums;

public enum Gender
{
    [XmlEnum("female")]
    [JsonProperty("female")]
    Female,

    [XmlEnum("male")]
    [JsonProperty("male")]
    Male
}
