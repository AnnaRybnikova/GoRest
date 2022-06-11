using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Integration;

[SetUpFixture]
public class ReportFixture
{
    public static ConcurrentDictionary<string, TestDetails> TestResults { get; } = new();

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        if (TestResults.IsEmpty)
            return;

        var outputJson = JsonConvert.SerializeObject(TestResults.Values);
        var outputFileName = $"TestResult-{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.json";

        const string outputDirectoryName = "TestResults";
        var outputDirectoryPath = Path.Combine(Path.GetFullPath(@"../../../"), outputDirectoryName);
        if (!Directory.Exists(outputDirectoryPath))
        {
            _ = Directory.CreateDirectory(outputDirectoryPath);
        }

        var outputFilePath = Path.Combine(outputDirectoryPath, outputFileName);

        File.WriteAllText(outputFilePath, outputJson);
    }

    public record TestDetails(
        string Name,
        string Class,
        string? Method,
        string? Message,
        TestResult Result,
        string? StackTrace);

    public record TestResult(
        string Label,
        string Site,
        string Status);
}
