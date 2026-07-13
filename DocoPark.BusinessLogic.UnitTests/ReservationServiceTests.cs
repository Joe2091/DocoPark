using DocoPark.BusinessLogic.DTOs.Reservation;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.BusinessLogic.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using Moq;
using System.Linq.Expressions;
using System.Timers;

namespace DocoPark.BusinessLogic.UnitTests;

[TestFixture]
public class ReservationServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepo;
    private Mock<IVehicleRepository> _mockVehicleRepo;
    private Mock<IParkingSpotRepository> _mockSpotRepo;
    private Mock<IReservationRepository> _mockReservationRepo;
    private ReservationService _service;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockVehicleRepo = new Mock<IVehicleRepository>();
        _mockSpotRepo = new Mock<IParkingSpotRepository>();
        _mockReservationRepo = new Mock<IReservationRepository>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Vehicles).Returns(_mockVehicleRepo.Object);
        _mockUnitOfWork.Setup(u => u.ParkingSpots).Returns(_mockSpotRepo.Object);
        _mockUnitOfWork.Setup(u => u.Reservations).Returns(_mockReservationRepo.Object);

        _service = new ReservationService(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task CreateReservationAsync_ValidData_ReturnsReservation()
    {
        var user = new User { Id = 1, Name = "Joe" };
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };
        var spot = new ParkingSpot { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Available };
        var dto = new CreateReservationDto
        {
            UserId = 1,
            VehicleId = 1,
            ParkingSpotId = 1,
            ReservedFrom = new DateTime(2026, 7, 14, 10, 0, 0, DateTimeKind.Utc),
            ReservedTo = new DateTime(2026, 7, 14, 14, 0, 0, DateTimeKind.Utc)
        };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { spot });
        _mockReservationRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
            .ReturnsAsync(new List<Reservation>());

        var result = await _service.CreateReservationAsync(dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.LicensePlate, Is.EqualTo("ABC123"));
        Assert.That(result.SpotNumber, Is.EqualTo("A1"));
        Assert.That(result.Status, Is.EqualTo(ReservationStatus.Active));
        _mockReservationRepo.Verify(r => r.AddAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Test]
    public void CreateReservationAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        var dto = new CreateReservationDto
        {
            UserId = 99,
            VehicleId = 1,
            ParkingSpotId = 1,
            ReservedFrom = DateTime.UtcNow,
            ReservedTo = DateTime.UtcNow.AddHours(2)
        };

        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateReservationAsync(dto));
    }

    [Test]
    public void CreateReservationAsync_VehicleNotFound_ThrowsKeyNotFoundException()
    {
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new() { Id = 1 } });
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle>());

        var dto = new CreateReservationDto
        {
            UserId = 1,
            VehicleId = 99,
            ParkingSpotId = 1,
            ReservedFrom = DateTime.UtcNow,
            ReservedTo = DateTime.UtcNow.AddHours(2)
        };

        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateReservationAsync(dto));
    }

    [Test]
    public void CreateReservationAsync_SpotNotFound_ThrowsKeyNotFoundException()
    {
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new() { Id = 1 } });
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { new() { Id = 1 } });
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot>());

        var dto = new CreateReservationDto
        {
            UserId = 1,
            VehicleId = 1,
            ParkingSpotId = 99,
            ReservedFrom = DateTime.UtcNow,
            ReservedTo = DateTime.UtcNow.AddHours(2)
        };

        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateReservationAsync(dto));
    }

    [Test]
    public void CreateReservationAsync_DoubleBooking_ThrowsInvalidOperationException()
    {
        var from = new DateTime(2026, 7, 14, 10, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 7, 14, 14, 0, 0, DateTimeKind.Utc);
        var existingReservation = new Reservation
        {
            Id = 1,
            ParkingSpotId = 1,
            Status = ReservationStatus.Active,
            ReservedFrom = from.AddHours(-1),
            ReservedTo = from.AddHours(2)
        };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new() { Id = 1 } });
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { new() { Id = 1 } });
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { new() { Id = 1, SpotNumber = "A1" } });
        _mockReservationRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
            .ReturnsAsync(new List<Reservation> { existingReservation });

        var dto = new CreateReservationDto
        {
            UserId = 1,
            VehicleId = 1,
            ParkingSpotId = 1,
            ReservedFrom = from,
            ReservedTo = to
        };

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateReservationAsync(dto));
    }

    [Test]
    public async Task GetReservationsByIdAsync_Exists_ReturnsReservation()
    {
        var reservation = new Reservation { Id = 1, UserId = 1, VehicleId = 1, ParkingSpotId = 1, Status = ReservationStatus.Active };

        _mockReservationRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { new() { Id = 1, LicensePlate = "ABC123" } });
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { new() { Id = 1, SpotNumber = "A1" } });

        var result = await _service.GetReservationsByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.LicensePlate, Is.EqualTo("ABC123"));
    }

    [Test]
    public async Task GetReservationsByIdAsync_NotFound_ReturnsNull()
    {
        _mockReservationRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Reservation?)null);

        var result = await _service.GetReservationsByIdAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CancelReservationAsync_Exists_ReturnsTrueAndFreesSpot()
    {
        var reservation = new Reservation { Id = 1, ParkingSpotId = 1, Status = ReservationStatus.Active };
        var spot = new ParkingSpot { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Reserved };

        _mockReservationRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot> { spot });

        var result = await _service.CancelReservationAsync(1);

        Assert.That(result, Is.True);
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
        Assert.That(spot.SpotStatus, Is.EqualTo(SpotStatus.Available));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CancelReservationAsync_NotFound_ReturnsFalse()
    {
        _mockReservationRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Reservation?)null);

        var result = await _service.CancelReservationAsync(99);

        Assert.That(result, Is.False);
    }
}