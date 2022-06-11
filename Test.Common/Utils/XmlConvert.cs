using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Test.Common.Utils;

public static class XmlConvert
{
    public static string SerializeObject<T>(T obj) where T : class
    {
        var xsSubmit = new XmlSerializer(typeof(T));
        using var sw = new StringWriter();
        using var writer = new XmlTextWriter(sw) { Formatting = Formatting.Indented };
        xsSubmit.Serialize(writer, obj);
        return sw.ToString();
    }

    public static T? DeserializeObject<T>(string str) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using var sr = new StringReader(str);

#pragma warning disable CA5369
        return (T?)serializer.Deserialize(sr);
#pragma warning restore CA5369
    }
}
