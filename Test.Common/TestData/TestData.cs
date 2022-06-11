using System.Collections.Generic;
using Test.Common.Domain.Data;

namespace Test.Common.TestData;

public class TestData
{
    public TestEntries<Comment> Comment { get; } = new();

    public TestEntries<Post> Post { get; } = new();

    public TestEntries<User> User { get; } = new();

    public TestEntries<User> InvalidUser { get; } = new();

    public TestEntries<Post> InvalidPost { get; } = new();

    public TestEntries<Comment> InvalidComment { get; } = new();

    public List<Post> Posts { get; } = new();

    public List<Comment> Comments { get; } = new();

    public string QueryParameter { get; set; }
}

public class TestEntries<T> where T : class
{
    public T ForCreate { get; set; }

    public T ForUpdate { get; set; }
}
