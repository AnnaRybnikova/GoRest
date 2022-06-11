using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.Common.Options;
using Test.Common.Services;
using Test.Common.Services.Implementation;
using Test.Common.Services.Rest;

namespace Test.Common;

public static class TestServices
{
    private static readonly IServiceCollection _services;

    private static IConfiguration Configuration { get; }

    public static ServiceProvider ServiceProvider { get; }

    public static IRestClientFactory RestClientFactory { get; }

    public static IRestClientFactory UnauthorizedRestClientFactory { get; }

    static TestServices()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile(@"Properties/appsettings.json")
            .Build();

        _services = new ServiceCollection();
        ConfigureServices(_services);

        ServiceProvider = _services.BuildServiceProvider();
        RestClientFactory = GetRequiredService<IRestClientFactory>();
        UnauthorizedRestClientFactory = RestClientFactory.Unauthorized();
    }

    public static T GetRequiredService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.Configure<TestServicesOptions>(Configuration.GetSection("TestServicesOptions"));
        services.AddSingleton<IRestClientFactory, RestClientFactory>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ITestContext, TestContext>();
    }
}

public static class ServiceCollectionExtensions
{
    public static IUserService Users(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IUserService>();
    }

    public static ICommentService Comments(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ICommentService>();
    }

    public static IPostService Posts(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IPostService>();
    }
}
