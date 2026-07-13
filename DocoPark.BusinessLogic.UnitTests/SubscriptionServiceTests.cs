using DocoPark.BusinessLogic.DTOs.Subscription;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.BusinessLogic.Services;
using DocoPark.Domain.Constants;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using Moq;
using System.Linq.Expressions;
using System.Timers;

namespace DocoPark.BusinessLogic.UnitTests;

[TestFixture]
public sealed class SubscriptionServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepo;
    private Mock<ISubscriptionRepository> _mockSubRepo;
    private SubscriptionService _service;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockSubRepo = new Mock<ISubscriptionRepository>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Subscriptions).Returns(_mockSubRepo.Object);

        _service = new SubscriptionService(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task CreateSubscriptionAsync_MonthlyType_SetsPremiumAndMonthlyFee()
    {
        var user = new User { Id = 1, Name = "Joe", IsPremium = false, SubscriptionType = SubscriptionType.Hourly };
        var dto = new CreateSubscriptionDto
        {
            UserId = 1,
            Type = SubscriptionType.Monthly,
            StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        _mockSubRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription>());

        var result = await _service.CreateSubscriptionAsync(dto);

        Assert.That(result.Type, Is.EqualTo(SubscriptionType.Monthly));
        Assert.That(result.MonthlyFee, Is.EqualTo(PricingConstants.MonthlyFee));
        Assert.That(result.IsActive, Is.True);
        Assert.That(user.IsPremium, Is.True);
        Assert.That(user.SubscriptionType, Is.EqualTo(SubscriptionType.Monthly));
        _mockSubRepo.Verify(r => r.AddAsync(It.IsAny<Subscription>()), Times.Once);
    }

    [Test]
    public async Task CreateSubscriptionAsync_HourlyType_NoMonthlyFee_NotPremium()
    {
        var user = new User { Id = 1, Name = "Joe", IsPremium = false };
        var dto = new CreateSubscriptionDto
        {
            UserId = 1,
            Type = SubscriptionType.Hourly,
            StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        _mockSubRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription>());

        var result = await _service.CreateSubscriptionAsync(dto);

        Assert.That(result.MonthlyFee, Is.EqualTo(0m));
        Assert.That(user.IsPremium, Is.False);
        Assert.That(user.SubscriptionType, Is.EqualTo(SubscriptionType.Hourly));
    }

    [Test]
    public async Task CreateSubscriptionAsync_DailyType_NoMonthlyFee()
    {
        var user = new User { Id = 1, Name = "Joe" };
        var dto = new CreateSubscriptionDto
        {
            UserId = 1,
            Type = SubscriptionType.Daily,
            StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        _mockSubRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription>());

        var result = await _service.CreateSubscriptionAsync(dto);

        Assert.That(result.MonthlyFee, Is.EqualTo(0m));
        Assert.That(result.Type, Is.EqualTo(SubscriptionType.Daily));
    }

    [Test]
    public void CreateSubscriptionAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        var dto = new CreateSubscriptionDto { UserId = 99, Type = SubscriptionType.Hourly, StartDate = DateTime.UtcNow };

        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateSubscriptionAsync(dto));
    }

    [Test]
    public void CreateSubscriptionAsync_AlreadyHasActiveSubscription_ThrowsInvalidOperationException()
    {
        var user = new User { Id = 1, Name = "Joe" };
        var activeSub = new Subscription { Id = 1, UserId = 1, IsActive = true };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        _mockSubRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription> { activeSub });

        var dto = new CreateSubscriptionDto { UserId = 1, Type = SubscriptionType.Monthly, StartDate = DateTime.UtcNow };

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateSubscriptionAsync(dto));
    }

    [Test]
    public async Task GetSubscriptionByIdAsync_Exists_ReturnsSubscription()
    {
        var sub = new Subscription { Id = 1, UserId = 1, Type = SubscriptionType.Monthly, IsActive = true, MonthlyFee = 150m };

        _mockSubRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sub);
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new() { Id = 1, Name = "Joe" } });

        var result = await _service.GetSubscriptionByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserName, Is.EqualTo("Joe"));
        Assert.That(result.MonthlyFee, Is.EqualTo(150m));
    }

    [Test]
    public async Task GetSubscriptionByIdAsync_NotFound_ReturnsNull()
    {
        _mockSubRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Subscription?)null);

        var result = await _service.GetSubscriptionByIdAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CancelSubscriptionAsync_MonthlySubscription_RemovesPremiumStatus()
    {
        var user = new User { Id = 1, Name = "Joe", IsPremium = true };
        var sub = new Subscription { Id = 1, UserId = 1, Type = SubscriptionType.Monthly, IsActive = true };

        _mockSubRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sub);
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        var result = await _service.CancelSubscriptionAsync(1);

        Assert.That(result, Is.True);
        Assert.That(sub.IsActive, Is.False);
        Assert.That(sub.EndDate, Is.Not.Null);
        Assert.That(user.IsPremium, Is.False);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CancelSubscriptionAsync_HourlySubscription_DoesNotTouchPremium()
    {
        var sub = new Subscription { Id = 1, UserId = 1, Type = SubscriptionType.Hourly, IsActive = true };

        _mockSubRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sub);

        var result = await _service.CancelSubscriptionAsync(1);

        Assert.That(result, Is.True);
        Assert.That(sub.IsActive, Is.False);
        // User repo should NOT be called for non-monthly
        _mockUserRepo.Verify(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);
    }

    [Test]
    public async Task CancelSubscriptionAsync_NotFound_ReturnsFalse()
    {
        _mockSubRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Subscription?)null);

        var result = await _service.CancelSubscriptionAsync(99);

        Assert.That(result, Is.False);
    }
}