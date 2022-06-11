using System;
using Test.Common.Domain.Data;
using Test.Common.Domain.Data.Requests;

namespace Test.Common.TestData;

public abstract class PatchTestData<T, TPatchRequest> : TestData where T : class where TPatchRequest : class
{
    public Action<TPatchRequest> PatchAction { get; set; }

    public Func<T, bool> PatchAssertion { get; set; }
}

public class PatchUserTestData : PatchTestData<User, UserPatchRequest>
{
}

public class PatchPostTestData : PatchTestData<Post, PostPatchRequest>
{
}

public class PatchCommentTestData : PatchTestData<Comment, CommentPatchRequest>
{
}
