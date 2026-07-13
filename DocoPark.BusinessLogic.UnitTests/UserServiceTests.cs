using System.Linq.Expressions;
using AutoMapper;
using DocoPark.BusinessLogic.DTOs.User;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.BusinessLogic.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using Moq;

namespace DocoPark.BusinessLogic.UnitTests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepo;
    private Mock<IMapper> _mockMapper;
    private UserService _service;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);

        _service = new UserService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Test]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Joe" },
            new() { Id = 2, Name = "Jane" }
        };
        var expectedDtos = new List<UserResponseDto>
        {
            new() { Id = 1, Name = "Joe" },
            new() { Id = 2, Name = "Jane" }
        };

        _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(expectedDtos);

        var result = (await _service.GetAllUsersAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Joe"));
    }

    [Test]
    public async Task GetUserByIdAsync_Exists_ReturnsUser()
    {
        var user = new User { Id = 1, Name = "Joe", Email = "joe@test.com" };
        var dto = new UserResponseDto { Id = 1, Name = "Joe", Email = "joe@test.com" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserResponseDto>(user)).Returns(dto);

        var result = await _service.GetUserByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Joe"));
    }

    [Test]
    public async Task GetUserByIdAsync_NotFound_ReturnsNull()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.GetUserByIdAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateUserAsync_ValidDto_ReturnsCreatedUser()
    {
        var createDto = new CreateUserDto { Name = "Joe", Email = "joe@test.com", Phone = "123456" };
        var user = new User { Id = 1, Name = "Joe", Email = "joe@test.com", Phone = "123456" };
        var responseDto = new UserResponseDto { Id = 1, Name = "Joe", Email = "joe@test.com" };

        _mockMapper.Setup(m => m.Map<User>(createDto)).Returns(user);
        _mockMapper.Setup(m => m.Map<UserResponseDto>(user)).Returns(responseDto);

        var result = await _service.CreateUserAsync(createDto);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Joe"));
        _mockUserRepo.Verify(r => r.AddAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateUserAsync_Exists_ReturnsUpdatedUser()
    {
        var user = new User { Id = 1, Name = "Joe", Email = "joe@test.com" };
        var updateDto = new UpdateUserDto { Name = "Joseph", Email = "joseph@test.com", Phone = "999" };
        var responseDto = new UserResponseDto { Id = 1, Name = "Joseph", Email = "joseph@test.com" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map(updateDto, user));
        _mockMapper.Setup(m => m.Map<UserResponseDto>(user)).Returns(responseDto);

        var result = await _service.UpdateUserAsync(1, updateDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Joseph"));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateUserAsync_NotFound_ReturnsNull()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.UpdateUserAsync(99, new UpdateUserDto());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteUserAsync_Exists_ReturnsTrue()
    {
        var user = new User { Id = 1, Name = "Joe" };
        _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _service.DeleteUserAsync(1);

        Assert.That(result, Is.True);
        _mockUserRepo.Verify(r => r.Remove(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteUserAsync_NotFound_ReturnsFalse()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.DeleteUserAsync(99);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetUsersBySubscriptionTypeAsync_ValidType_ReturnsUsers()
    {
        var users = new List<User> { new() { Id = 1, Name = "Joe", SubscriptionType = SubscriptionType.Monthly } };
        var dtos = new List<UserResponseDto> { new() { Id = 1, Name = "Joe", SubscriptionType = SubscriptionType.Monthly } };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(dtos);

        var result = (await _service.GetUsersBySubscriptionTypeAsync("Monthly")).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].SubscriptionType, Is.EqualTo(SubscriptionType.Monthly));
    }

    [Test]
    public async Task GetUsersBySubscriptionTypeAsync_InvalidType_ReturnsEmpty()
    {
        var result = (await _service.GetUsersBySubscriptionTypeAsync("InvalidType")).ToList();

        Assert.That(result, Is.Empty);
    }
}