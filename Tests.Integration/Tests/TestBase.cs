using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Test.Common;

namespace Tests.Integration.Tests;

[TestFixture]
public abstract class TestBase
{
    private AsyncServiceScope _scope;

    protected IServiceProvider Services { get; private set; }

    [SetUp]
    public void Setup()
    {
        _scope = TestServices.ServiceProvider.CreateAsyncScope();
        Services = _scope.ServiceProvider;
    }

    [TearDown]
    public async Task Cleanup()
    {
        var test = TestContext.CurrentContext.Test;
        var testResult = TestContext.CurrentContext.Result;

        if (test is { Name: not null, ClassName: not null })
        {
            ReportFixture.TestResults.TryAdd(test.Name,
                new ReportFixture.TestDetails(
                    test.Name,
                    test.ClassName,
                    test.MethodName,
                    testResult.Message,
                    new ReportFixture.TestResult(
                        testResult.Outcome.Label,
                        testResult.Outcome.Site.ToString(),
                        testResult.Outcome.Status.ToString()
                        ),
                    testResult.StackTrace));
        }

        await _scope.DisposeAsync();
    }
}
