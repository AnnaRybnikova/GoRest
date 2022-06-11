## README
This solution allows you to test the https://gorest.co.in/ resource by its main endpoints and nested resources.

During the tests, a bug was found on the tested resource, namely, a search for the body. With a given search, instead of displaying posts/comments with this content, we returned to the main page.

This test script was skipped out, if you need to check it, just remove the ".Ignore("issue with GoRest, query by body doesn't work")" in the CreateTestData.cs class.

Also added the ability to select Encoding, but it is not working due to lack of documentation from Gorest.
After testing is completed, reports in the .json format are generated in the TestResults folder, which allows you to check the correct operation of the service.

It is possible to run all tests using the following command from the project root: _dotnet test .\Tests.Integration\Tests.Integration.csproj_

If necessary, you can run tests of only a certain category (there are 5 of them in this project: Get, Put, Post, Patch, Delete)
- _--filter "TestCategory = Name"_

It is also possible to run tests of only one of the files (NegativePostTest, NegativeUserTest, PostsTest, UsersTest, NegativeCommentTest, CommentTest, ComplexTest)
- _--filter Name~NameFile_

Example of one complete command for NegativePostTest tests with category Get: _dotnet test .\Tests.Integration\Tests.Integration.csproj --filter "Name~NegativePostTest&TestCategory=Get"_

When running tests, you can use the built-in generator for creating a test result file in the TRX format, if the received reports in the .json format do not meet your needs, for this it is enough to add _--logger trx_ at the end of the main command.

If you need to see the execution log in the console, use _--logger "console;verbosity=detailed"_
