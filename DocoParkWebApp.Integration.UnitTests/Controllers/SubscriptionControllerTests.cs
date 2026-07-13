using DocoPark.BusinessLogic.DTOs.Subscription;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Enums;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Controllers;

[TestFixture]
public class SubscriptionsControllerTests
{
    private Mock<ISubscriptionService> _mockSubscriptionService;
    private SubscriptionsController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockSubscriptionService = new Mock<ISubscriptionService>();
        var logger = new Mock<ILogger<SubscriptionsController>>().Object;
        _controller = new SubscriptionsController(_mockSubscriptionService.Object, logger);
    }

    [Test]
    public async Task Create_ReturnsCreatedResult_WhenValid()
    {
        // Arrange
        var dto = new CreateSubscriptionDto
        {
            UserId = 1,
            Type = SubscriptionType.Monthly,
            StartDate = DateTime.UtcNow
        };
        var response = new SubscriptionResponseDto
        {
            Id = 1,
            UserId = 1,
            Type = SubscriptionType.Monthly,
            MonthlyFee = 150.00m,
            IsActive = true,
            UserName = "Alice"
        };
        _mockSubscriptionService.Setup(s => s.CreateSubscriptionAsync(dto)).ReturnsAsync(response);

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var createdResult = actionResult.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenSubscriptionExists()
    {
        // Arrange
        var subscription = new SubscriptionResponseDto
        {
            Id = 1,
            UserId = 1,
            Type = SubscriptionType.Monthly,
            IsActive = true,
            UserName = "Alice"
        };
        _mockSubscriptionService.Setup(s => s.GetSubscriptionByIdAsync(1)).ReturnsAsync(subscription);

        // Act
        var actionResult = await _controller.GetById(1);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenSubscriptionDoesNotExist()
    {
        // Arrange
        _mockSubscriptionService.Setup(s => s.GetSubscriptionByIdAsync(99)).ReturnsAsync((SubscriptionResponseDto?)null);

        // Act
        var actionResult = await _controller.GetById(99);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task Cancel_ReturnsNoContent_WhenSubscriptionExists()
    {
        // Arrange
        _mockSubscriptionService.Setup(s => s.CancelSubscriptionAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Cancel(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Cancel_ReturnsNotFound_WhenSubscriptionDoesNotExist()
    {
        // Arrange
        _mockSubscriptionService.Setup(s => s.CancelSubscriptionAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Cancel(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetActive_ReturnsOkResult_WithActiveSubscriptions()
    {
        // Arrange
        var subscriptions = new List<SubscriptionResponseDto>
        {
            new() { Id = 1, Type = SubscriptionType.Monthly, IsActive = true, UserName = "Alice" },
            new() { Id = 2, Type = SubscriptionType.Hourly, IsActive = true, UserName = "Bob" }
        };
        _mockSubscriptionService.Setup(s => s.GetActiveSubscriptionsAsync()).ReturnsAsync(subscriptions);

        // Act
        var actionResult = await _controller.GetActive();

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returned = okResult!.Value as IEnumerable<SubscriptionResponseDto>;
        Assert.That(returned!.Count(), Is.EqualTo(2));
    }
}