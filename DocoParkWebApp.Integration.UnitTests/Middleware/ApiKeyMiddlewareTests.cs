using DocoParkWebApp.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Middleware;

[TestFixture]
public class ApiKeyMiddlewareTests
{
    private const string ValidApiKey = "test-api-key-123";
    private Mock<IConfiguration> _mockConfiguration;
    private bool _nextWasCalled;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["ApiKey"]).Returns(ValidApiKey);
        _nextWasCalled = false;
    }

    private ApiKeyMiddleware CreateMiddleware()
    {
        return new ApiKeyMiddleware(
            next: _ => { _nextWasCalled = true; return Task.CompletedTask; },
            _mockConfiguration.Object);
    }

    private static DefaultHttpContext CreateContext(string path = "/api/users", string? apiKey = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Response.Body = new MemoryStream();

        if (apiKey is not null)
        {
            context.Request.Headers["X-Api-Key"] = apiKey;
        }

        return context;
    }

    [Test]
    public async Task InvokeAsync_ValidApiKey_PassesThrough()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(apiKey: ValidApiKey);

        await middleware.InvokeAsync(context);

        Assert.That(_nextWasCalled, Is.True);
    }

    [Test]
    public async Task InvokeAsync_MissingApiKey_Returns401()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(apiKey: null);

        await middleware.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(_nextWasCalled, Is.False);
    }

    [Test]
    public async Task InvokeAsync_InvalidApiKey_Returns401()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(apiKey: "wrong-key");

        await middleware.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(_nextWasCalled, Is.False);
    }

    [Test]
    public async Task InvokeAsync_SwaggerPath_BypassesAuth()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(path: "/swagger/index.html", apiKey: null);

        await middleware.InvokeAsync(context);

        Assert.That(_nextWasCalled, Is.True);
    }

    [Test]
    public async Task InvokeAsync_RootPath_BypassesAuth()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(path: "/", apiKey: null);

        await middleware.InvokeAsync(context);

        Assert.That(_nextWasCalled, Is.True);
    }

    [Test]
    public async Task InvokeAsync_MissingApiKey_ResponseContainsMissingMessage()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(apiKey: null);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.That(body, Does.Contain("API Key is missing"));
    }

    [Test]
    public async Task InvokeAsync_InvalidApiKey_ResponseContainsInvalidMessage()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(apiKey: "bad-key");

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.That(body, Does.Contain("Invalid API Key"));
    }

    [Test]
    public async Task InvokeAsync_SwaggerSubPath_BypassesAuth()
    {
        var middleware = CreateMiddleware();
        var context = CreateContext(path: "/swagger/v1/swagger.json", apiKey: null);

        await middleware.InvokeAsync(context);

        Assert.That(_nextWasCalled, Is.True);
    }
}