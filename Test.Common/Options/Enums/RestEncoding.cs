using System.Runtime.Serialization;

namespace Test.Common.Options.Enums;

public enum RestEncoding
{
    /// <summary>
    /// Default is json (without specifying in endpoint)
    /// </summary>
    [EnumMember(Value = "default")]
    Default,

    /// <summary>
    /// Json with specifying in endpoint ".json"
    /// </summary>
    [EnumMember(Value = "json")]
    Json,

    /// <summary>
    /// Xml with specifying in endpoint ".xml"
    /// </summary>
    [EnumMember(Value = "xml")]
    Xml
}
