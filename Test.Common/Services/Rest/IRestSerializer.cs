namespace Test.Common.Services.Rest;

public interface IRestSerializer
{
    string Serialize<T>(T data) where T : class;

    T? Deserialize<T>(string str) where T : class;
}
