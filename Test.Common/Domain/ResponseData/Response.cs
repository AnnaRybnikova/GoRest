using Test.Common.Domain.Meta;

namespace Test.Common.Domain.ResponseData;

public class Response<T> where T : class
{
    public T Data { get; set; }
}

public class CollectionResponse<T> : Response<T[]> where T : class
{
    public MetaData Meta { get; set; }
}
