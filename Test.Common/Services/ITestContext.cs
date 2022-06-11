using System;
using System.Collections.Concurrent;
using Test.Common.Domain.Data;

namespace Test.Common.Services;

public interface ITestContext : IAsyncDisposable
{
    ConcurrentDictionary<int, User> CreatedUsers { get; }

    ConcurrentDictionary<int, Post> CreatedPosts { get; }

    ConcurrentDictionary<int, Comment> CreatedComments { get; }
}
