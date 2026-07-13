using DocoPark.BusinessLogic.DTOs.User;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Enums;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Controllers;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUserService> _mockUserService;
    private UsersController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockUserService = new Mock<IUserService>();
        var logger = new Mock<ILogger<UsersController>>().Object;
        _controller = new UsersController(_mockUserService.Object, logger);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfUsers()
    {
        // Arrange
        var users = new List<UserResponseDto>
        {
            new() { Id = 1, Name = "Alice", Email = "alice@example.com" },
            new() { Id = 2, Name = "Bob", Email = "bob@example.com" }
        };
        _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedUsers = okResult!.Value as IEnumerable<UserResponseDto>;
        Assert.That(returnedUsers, Is.Not.Null);
        Assert.That(returnedUsers!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<UserResponseDto>());

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedUsers = okResult!.Value as IEnumerable<UserResponseDto>;
        Assert.That(returnedUsers, Is.Not.Null);
        Assert.That(returnedUsers!, Is.Empty);
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var user = new UserResponseDto { Id = 1, Name = "Alice", Email = "alice@example.com" };
        _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var actionResult = await _controller.GetById(1);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedUser = okResult!.Value as UserResponseDto;
        Assert.That(returnedUser, Is.Not.Null);
        Assert.That(returnedUser!.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetUserByIdAsync(99)).ReturnsAsync((UserResponseDto?)null);

        // Act
        var actionResult = await _controller.GetById(99);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }
}