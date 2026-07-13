using System.Linq.Expressions;
using DocoPark.BusinessLogic.DTOs.ParkingSession;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.BusinessLogic.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using Moq;

namespace DocoPark.BusinessLogic.UnitTests;

[TestFixture]
public sealed class ParkingSessionServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPricingService> _mockPricingService;
    private Mock<IVehicleRepository> _mockVehicleRepo;
    private Mock<IParkingSessionRepository> _mockSessionRepo;
    private Mock<IParkingSpotRepository> _mockSpotRepo;
    private ParkingSessionService _service;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPricingService = new Mock<IPricingService>();
        _mockVehicleRepo = new Mock<IVehicleRepository>();
        _mockSessionRepo = new Mock<IParkingSessionRepository>();
        _mockSpotRepo = new Mock<IParkingSpotRepository>();

        _mockUnitOfWork.Setup(u => u.Vehicles).Returns(_mockVehicleRepo.Object);
        _mockUnitOfWork.Setup(u => u.ParkingSessions).Returns(_mockSessionRepo.Object);
        _mockUnitOfWork.Setup(u => u.ParkingSpots).Returns(_mockSpotRepo.Object);

        _service = new ParkingSessionService(_mockUnitOfWork.Object, _mockPricingService.Object);
    }

    [Test]
    public async Task CheckInAsync_ValidPlate_ReturnsSession()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123", UserId = 1 };
        var spot = new ParkingSpot { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Available };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSessionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSession, bool>>>()))
            .ReturnsAsync(new List<ParkingSession>());
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { spot });

        var result = await _service.CheckInAsync(new CheckInRequestDto { LicensePlate = "ABC123" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.LicensePlate, Is.EqualTo("ABC123"));
        Assert.That(result.SpotNumber, Is.EqualTo("A1"));
        _mockSessionRepo.Verify(r => r.AddAsync(It.IsAny<ParkingSession>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Test]
    public void CheckInAsync_VehicleNotFound_ThrowsKeyNotFoundException()
    {
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle>());

        Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CheckInAsync(new CheckInRequestDto { LicensePlate = "UNKNOWN" }));
    }

    [Test]
    public void CheckInAsync_AlreadyCheckedIn_ThrowsInvalidOperationException()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };
        var activeSession = new ParkingSession { Id = 1, VehicleId = 1, CheckOutTime = null };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSessionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSession, bool>>>()))
            .ReturnsAsync(new List<ParkingSession> { activeSession });

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CheckInAsync(new CheckInRequestDto { LicensePlate = "ABC123" }));
    }

    [Test]
    public void CheckInAsync_NoAvailableSpots_ThrowsInvalidOperationException()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSessionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSession, bool>>>()))
            .ReturnsAsync(new List<ParkingSession>());
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot>());

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CheckInAsync(new CheckInRequestDto { LicensePlate = "ABC123" }));
    }

    [Test]
    public async Task CheckOutAsync_ValidSession_ReturnsCostAndFreesSpot()
    {
        var vehicle = new Vehicle
        {
            Id = 1,
            LicensePlate = "ABC123",
            User = new User { Id = 1, Name = "Joe", SubscriptionType = SubscriptionType.Hourly }
        };
        var session = new ParkingSession { Id = 1, VehicleId = 1, ParkingSpotId = 1, CheckInTime = DateTime.UtcNow.AddHours(-2), CheckOutTime = null };
        var spot = new ParkingSpot { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Occupied };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSessionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSession, bool>>>()))
            .ReturnsAsync(new List<ParkingSession> { session });
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { spot });
        _mockPricingService.Setup(p => p.CalculateSessionCost(It.IsAny<DateTime>(), It.IsAny<DateTime>(), SubscriptionType.Hourly))
            .Returns(6.00m);

        var result = await _service.CheckOutAsync(new CheckOutRequestDto { LicensePlate = "ABC123" });

        Assert.That(result.TotalCost, Is.EqualTo(6.00m));
        Assert.That(result.SpotStatus, Is.EqualTo(SpotStatus.Available));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void CheckOutAsync_VehicleNotFound_ThrowsKeyNotFoundException()
    {
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle>());

        Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CheckOutAsync(new CheckOutRequestDto { LicensePlate = "UNKNOWN" }));
    }

    [Test]
    public void CheckOutAsync_NoActiveSession_ThrowsKeyNotFoundException()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSessionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSession, bool>>>()))
            .ReturnsAsync(new List<ParkingSession>());

        Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CheckOutAsync(new CheckOutRequestDto { LicensePlate = "ABC123" }));
    }

    [Test]
    public async Task GetSessionByIdAsync_Exists_ReturnsSession()
    {
        var session = new ParkingSession { Id = 1, VehicleId = 1, ParkingSpotId = 1 };
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };
        var spot = new ParkingSpot { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Occupied };

        _mockSessionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(session);
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { spot });

        var result = await _service.GetSessionByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.LicensePlate, Is.EqualTo("ABC123"));
    }

    [Test]
    public async Task GetSessionByIdAsync_NotFound_ReturnsNull()
    {
        _mockSessionRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ParkingSession?)null);

        var result = await _service.GetSessionByIdAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveSessionsAsync_ReturnsSessions()
    {
        var sessions = new List<ParkingSession>
        {
            new() { Id = 1, VehicleId = 1, ParkingSpotId = 1, CheckOutTime = null },
            new() { Id = 2, VehicleId = 2, ParkingSpotId = 2, CheckOutTime = null }
        };

        _mockSessionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSession, bool>>>()))
            .ReturnsAsync(sessions);
        _mockVehicleRepo.SetupSequence(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { new() { Id = 1, LicensePlate = "ABC123" } })
            .ReturnsAsync(new List<Vehicle> { new() { Id = 2, LicensePlate = "XYZ789" } });
        _mockSpotRepo.SetupSequence(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { new() { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Occupied } })
            .ReturnsAsync(new List<ParkingSpot> { new() { Id = 2, SpotNumber = "A2", SpotStatus = SpotStatus.Occupied } });

        var result = (await _service.GetActiveSessionsAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
    }
}