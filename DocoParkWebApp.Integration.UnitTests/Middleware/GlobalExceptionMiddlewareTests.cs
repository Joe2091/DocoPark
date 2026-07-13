using System.Net;
using DocoParkWebApp.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Middleware;

[TestFixture]
public class GlobalExceptionMiddlewareTests
{
    private Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
    private Mock<IHostEnvironment> _mockEnvironment;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _mockEnvironment = new Mock<IHostEnvironment>();
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
    }

    private DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var services = new ServiceCollection();
        services.AddSingleton(_mockEnvironment.Object);
        context.RequestServices = services.BuildServiceProvider();

        return context;
    }

    private async Task<(int StatusCode, string Body)> InvokeMiddlewareWithException(Exception exception)
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw exception,
            _mockLogger.Object);

        var context = CreateHttpContext();
        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        return (context.Response.StatusCode, body);
    }

    [Test]
    public async Task InvokeAsync_KeyNotFoundException_Returns404()
    {
        var (statusCode, body) = await InvokeMiddlewareWithException(new KeyNotFoundException("Not found"));

        Assert.That(statusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.That(body, Does.Contain("Resource not found"));
    }

    [Test]
    public async Task InvokeAsync_ArgumentException_Returns400()
    {
        var (statusCode, body) = await InvokeMiddlewareWithException(new ArgumentException("Bad arg"));

        Assert.That(statusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(body, Does.Contain("Invalid request"));
    }

    [Test]
    public async Task InvokeAsync_UnauthorizedAccessException_Returns401()
    {
        var (statusCode, body) = await InvokeMiddlewareWithException(new UnauthorizedAccessException("No access"));

        Assert.That(statusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        Assert.That(body, Does.Contain("Unauthorized"));
    }

    [Test]
    public async Task InvokeAsync_InvalidOperationException_Returns409()
    {
        var (statusCode, body) = await InvokeMiddlewareWithException(new InvalidOperationException("Conflict"));

        Assert.That(statusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
        Assert.That(body, Does.Contain("Operation not valid"));
    }

    [Test]
    public async Task InvokeAsync_GenericException_Returns500()
    {
        var (statusCode, body) = await InvokeMiddlewareWithException(new Exception("Something broke"));

        Assert.That(statusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.That(body, Does.Contain("unexpected error"));
    }

    [Test]
    public async Task InvokeAsync_NoException_PassesThrough()
    {
        var wasCalled = false;
        var middleware = new GlobalExceptionMiddleware(
            next: _ => { wasCalled = true; return Task.CompletedTask; },
            _mockLogger.Object);

        var context = CreateHttpContext();
        await middleware.InvokeAsync(context);

        Assert.That(wasCalled, Is.True);
    }

    [Test]
    public async Task InvokeAsync_DevelopmentEnvironment_IncludesDetails()
    {
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");

        var (_, body) = await InvokeMiddlewareWithException(new KeyNotFoundException("Specific detail"));

        Assert.That(body, Does.Contain("Specific detail"));
    }

    [Test]
    public async Task InvokeAsync_ProductionEnvironment_ExcludesDetails()
    {
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var (_, body) = await InvokeMiddlewareWithException(new KeyNotFoundException("Secret detail"));

        Assert.That(body, Does.Not.Contain("Secret detail"));
    }

    [Test]
    public async Task InvokeAsync_SetsJsonContentType()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new Exception("error"),
            _mockLogger.Object);

        var context = CreateHttpContext();
        await middleware.InvokeAsync(context);

        Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
    }
}