using System.Collections;
using NUnit.Framework;
using Test.Common.TestData;
using Test.Common.Utils;

namespace Tests.Integration.Tests;

internal class CreateTestData
{
    internal static IEnumerable ValidData
    {
        get
        {
            var data = new TestData
            {
                User =
                {
                    ForCreate = FakeData.CreateValidUser(),
                    ForUpdate = FakeData.CreateValidUser()
                },
                Post =
                {
                    ForCreate = FakeData.CreateValidPost(),
                    ForUpdate = FakeData.CreateValidPost()
                },
                Comment =
                {
                    ForCreate = FakeData.CreateValidComment(),
                    ForUpdate = FakeData.CreateValidComment()
                }
            };

            yield return new TestCaseData(data).SetArgDisplayNames("ValidData");
        }
    }

    internal static IEnumerable InvalidData
    {
        get
        {
            var data = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                Comment = { ForCreate = FakeData.CreateValidComment() },
                InvalidUser =
                {
                    ForCreate = FakeData.CreateEmptyUser(),
                    ForUpdate = FakeData.CreateEmptyUser()
                },
                InvalidPost =
                {
                    ForCreate = FakeData.CreateEmptyPost(),
                    ForUpdate = FakeData.CreateEmptyPost()
                },
                InvalidComment =
                {
                    ForCreate = FakeData.CreateEmptyComment(),
                    ForUpdate = FakeData.CreateEmptyComment()
                }
            };

            yield return new TestCaseData(data).SetArgDisplayNames("InvalidDataForSend");
        }
    }

    internal static IEnumerable ComplexTestPosts
    {
        get
        {
            var data = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() }
            };

            for (var i = 0; i < 21; i++)
            {
                data.Posts.Add(FakeData.CreateValidPost());
            }

            yield return new TestCaseData(data).SetArgDisplayNames("ComplexTestPosts");
        }
    }

    internal static IEnumerable ComplexTestComments
    {
        get
        {
            var data = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() }
            };

            for (var i = 0; i < 21; i++)
            {
                data.Comments.Add(FakeData.CreateValidComment());
            }

            yield return new TestCaseData(data).SetArgDisplayNames("ComplexTestComments");
        }
    }

    internal static IEnumerable UserPatchData
    {
        get
        {
            var data = new PatchUserTestData
            {
                User =
                {
                    ForCreate = FakeData.CreateValidUser(), ForUpdate = FakeData.CreateValidUser()
                }
            };
            data.PatchAction = user => user.Email = data.User.ForUpdate.Email;
            data.PatchAssertion = user => user.Email == data.User.ForUpdate.Email;

            yield return new TestCaseData(data).SetArgDisplayNames("PatchUserEmail");

            var data2 = new PatchUserTestData
            {
                User =
                {
                    ForCreate = FakeData.CreateValidUser(), ForUpdate = FakeData.CreateValidUser()
                },
            };
            data2.PatchAction = user => user.Name = data2.User.ForUpdate.Name;
            data2.PatchAssertion = user => user.Name == data2.User.ForUpdate.Name;

            yield return new TestCaseData(data2).SetArgDisplayNames("PatchUserName");
        }
    }

    internal static IEnumerable PostPatchData
    {
        get
        {
            var data = new PatchPostTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post =
                {
                    ForCreate = FakeData.CreateValidPost(), ForUpdate = FakeData.CreateValidPost()
                }
            };
            data.PatchAction = post => post.Title = data.Post.ForUpdate.Title;
            data.PatchAssertion = post => post.Title == data.Post.ForUpdate.Title;

            yield return new TestCaseData(data).SetArgDisplayNames("PatchPostTitle");
        }
    }

    internal static IEnumerable CommentPatchData
    {
        get
        {
            var data = new PatchCommentTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post =
                {
                    ForCreate = FakeData.CreateValidPost(), ForUpdate = FakeData.CreateValidPost()
                },
                Comment =
                {
                    ForCreate = FakeData.CreateValidComment(), ForUpdate = FakeData.CreateValidComment()
                }
            };
            data.PatchAction = comment => comment.Name = data.Comment.ForUpdate.Name;
            data.PatchAssertion = comment => comment.Name == data.Comment.ForUpdate.Name;

            yield return new TestCaseData(data).SetArgDisplayNames("PatchCommentName");

            var data2 = new PatchCommentTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post =
                {
                    ForCreate = FakeData.CreateValidPost(), ForUpdate = FakeData.CreateValidPost()
                },
                Comment =
                {
                    ForCreate = FakeData.CreateValidComment(), ForUpdate = FakeData.CreateValidComment()
                }
            };
            data2.PatchAction = comment => comment.Email = data2.Comment.ForUpdate.Email;
            data2.PatchAssertion = comment => comment.Email == data2.Comment.ForUpdate.Email;

            yield return new TestCaseData(data2).SetArgDisplayNames("PatchCommentEmail");
        }
    }

    internal static IEnumerable InvalidUserPatchData
    {
        get
        {
            var data = new PatchUserTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                InvalidUser = { ForUpdate = FakeData.CreateEmptyUser() }
            };
            data.PatchAction = user => user.Email = data.InvalidUser.ForUpdate.Email;
            data.PatchAssertion = user => user.Email == data.InvalidUser.ForUpdate.Email;

            yield return new TestCaseData(data).SetArgDisplayNames("PatchUserEmail");

            var data2 = new PatchUserTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                InvalidUser = { ForUpdate = FakeData.CreateEmptyUser() }
            };
            data2.PatchAction = user => user.Name = data2.InvalidUser.ForUpdate.Name;
            data2.PatchAssertion = user => user.Name == data2.InvalidUser.ForUpdate.Name;

            yield return new TestCaseData(data2).SetArgDisplayNames("PatchUserName");
        }
    }

    internal static IEnumerable InvalidPostPatchData
    {
        get
        {
            var data = new PatchPostTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                InvalidPost = { ForUpdate = FakeData.CreateEmptyPost() }
            };
            data.PatchAction = post => post.Title = data.InvalidPost.ForUpdate.Title;
            data.PatchAssertion = post => post.Title == data.InvalidPost.ForUpdate.Title;

            yield return new TestCaseData(data).SetArgDisplayNames("PatchPostTitle");
        }
    }

    internal static IEnumerable InvalidCommentPatchData
    {
        get
        {
            var data = new PatchCommentTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                Comment = { ForCreate = FakeData.CreateValidComment() },
                InvalidComment = { ForUpdate = FakeData.CreateEmptyComment() }
            };
            data.PatchAction = comment => comment.Name = data.InvalidComment.ForUpdate.Name;
            data.PatchAssertion = comment => comment.Name == data.InvalidComment.ForUpdate.Name;

            yield return new TestCaseData(data).SetArgDisplayNames("InvalidCommentPatchName");

            var data2 = new PatchCommentTestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                Comment = { ForCreate = FakeData.CreateValidComment() },
                InvalidComment = { ForUpdate = FakeData.CreateEmptyComment() }
            };
            data2.PatchAction = comment => comment.Email = data.InvalidComment.ForUpdate.Email;
            data2.PatchAssertion = comment => comment.Email == data.InvalidComment.ForUpdate.Email;

            yield return new TestCaseData(data2).SetArgDisplayNames("InvalidCommentPatchEmail");
        }
    }

    internal static IEnumerable ExistingUserEmailPatchData
    {
        get
        {
            var data = new PatchUserTestData
            {
                User =
                {
                    ForCreate = FakeData.CreateValidUser(), ForUpdate = FakeData.CreateValidUser()
                },
            };
            data.PatchAction = user => user.Email = data.User.ForUpdate.Email;
            data.PatchAssertion = user => user.Email == data.User.ForUpdate.Email;

            yield return new TestCaseData(data).SetArgDisplayNames("PatchUserEmailAlreadyExist");
        }
    }

    internal static IEnumerable ValidDataWithQueryParamsForUser
    {
        get
        {
            var data1 = new TestData
            {
                User =
                {
                    ForCreate = FakeData.CreateValidUser(), ForUpdate = FakeData.CreateValidUser()
                }
            };
            data1.QueryParameter = $"name={data1.User.ForCreate.Name}";

            yield return new TestCaseData(data1).SetArgDisplayNames("GetByName");

            var data2 = new TestData
            {
                User =
                {
                    ForCreate = FakeData.CreateValidUser(), ForUpdate = FakeData.CreateValidUser()
                }
            };
            data2.QueryParameter = $"email={data2.User.ForCreate.Email}";

            yield return new TestCaseData(data2).SetArgDisplayNames("GetByEmail");
        }
    }

    internal static IEnumerable ValidDataWithQueryParamsForPost
    {
        get
        {
            var data1 = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post =
                {
                    ForCreate = FakeData.CreateValidPost(), ForUpdate = FakeData.CreateValidPost()
                }
            };
            data1.QueryParameter = $"title={data1.Post.ForCreate.Title}";

            yield return new TestCaseData(data1).SetArgDisplayNames("GetByTitle");

            var data2 = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post =
                {
                    ForCreate = FakeData.CreateValidPost(), ForUpdate = FakeData.CreateValidPost()
                }
            };
            data2.QueryParameter = $"body={data2.Post.ForCreate.Body}";

            yield return new TestCaseData(data2)
                .Ignore("issue with GoRest, query by body doesn't work")
                .SetArgDisplayNames("GetByBody");
        }
    }

    internal static IEnumerable ValidDataWithQueryParamsForComments
    {
        get
        {
            var data1 = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                Comment =
                {
                    ForCreate = FakeData.CreateValidComment(), ForUpdate = FakeData.CreateValidComment()
                }
            };
            data1.QueryParameter = $"name={data1.Comment.ForCreate.Name}";

            yield return new TestCaseData(data1).SetArgDisplayNames("GetByName");

            var data2 = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                Comment =
                {
                    ForCreate = FakeData.CreateValidComment(), ForUpdate = FakeData.CreateValidComment()
                }
            };
            data2.QueryParameter = $"email={data2.Comment.ForCreate.Email}";

            yield return new TestCaseData(data2).SetArgDisplayNames("GetByEmail");

            var data3 = new TestData
            {
                User = { ForCreate = FakeData.CreateValidUser() },
                Post = { ForCreate = FakeData.CreateValidPost() },
                Comment =
                {
                    ForCreate = FakeData.CreateValidComment(), ForUpdate = FakeData.CreateValidComment()
                }
            };
            data3.QueryParameter = $"body={data3.Comment.ForCreate.Body}";

            yield return new TestCaseData(data3)
                .Ignore("issue with GoRest, query by body doesn't work")
                .SetArgDisplayNames("GetByBody");
        }
    }
}
