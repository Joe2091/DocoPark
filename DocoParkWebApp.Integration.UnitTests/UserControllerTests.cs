using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.Domain.Entities;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocoPark.Tests.Controllers;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepo;
    private UsersController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _controller = new UsersController(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Name = "Alice", Email = "alice@example.com" },
            new() { Id = 2, Name = "Bob", Email = "bob@example.com" }
        };
        _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var returnedUsers = okResult.Value as IEnumerable<User>;
        Assert.That(returnedUsers, Is.Not.Null);
        Assert.That(returnedUsers!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var returnedUsers = okResult.Value as IEnumerable<User>;
        Assert.That(returnedUsers, Is.Not.Null);
        Assert.That(returnedUsers!, Is.Empty);
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com" };
        _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var returnedUser = okResult.Value as User;
        Assert.That(returnedUser, Is.Not.Null);
        Assert.That(returnedUser!.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}