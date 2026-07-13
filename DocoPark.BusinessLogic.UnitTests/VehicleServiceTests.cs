using System.Linq.Expressions;
using AutoMapper;
using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.BusinessLogic.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using Moq;

namespace DocoPark.BusinessLogic.UnitTests;

[TestFixture]
public sealed class VehicleServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IVehicleRepository> _mockVehicleRepo;
    private Mock<IMapper> _mockMapper;
    private VehicleService _service;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockVehicleRepo = new Mock<IVehicleRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(u => u.Vehicles).Returns(_mockVehicleRepo.Object);

        _service = new VehicleService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Test]
    public async Task GetAllVehiclesAsync_ReturnsAllVehicles()
    {
        var vehicles = new List<Vehicle>
        {
            new() { Id = 1, LicensePlate = "ABC123", Color = "Red", VehicleType = VehicleType.Car },
            new() { Id = 2, LicensePlate = "XYZ789", Color = "Blue", VehicleType = VehicleType.Motorcycle }
        };
        var expectedDtos = new List<VehicleResponseDto>
        {
            new() { Id = 1, LicensePlate = "ABC123", Color = "Red" },
            new() { Id = 2, LicensePlate = "XYZ789", Color = "Blue" }
        };

        _mockVehicleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(vehicles);
        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleResponseDto>>(vehicles)).Returns(expectedDtos);

        var result = (await _service.GetAllVehiclesAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetVehicleByIdAsync_Exists_ReturnsVehicle()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123", Color = "Red" };
        var dto = new VehicleResponseDto { Id = 1, LicensePlate = "ABC123", Color = "Red" };

        _mockVehicleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);
        _mockMapper.Setup(m => m.Map<VehicleResponseDto>(vehicle)).Returns(dto);

        var result = await _service.GetVehicleByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.LicensePlate, Is.EqualTo("ABC123"));
    }

    [Test]
    public async Task GetVehicleByIdAsync_NotFound_ReturnsNull()
    {
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Vehicle?)null);

        var result = await _service.GetVehicleByIdAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetVehiclesByUserIdAsync_ReturnsUserVehicles()
    {
        var vehicles = new List<Vehicle> { new() { Id = 1, LicensePlate = "ABC123", UserId = 1 } };
        var dtos = new List<VehicleResponseDto> { new() { Id = 1, LicensePlate = "ABC123", UserId = 1 } };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(vehicles);
        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleResponseDto>>(vehicles)).Returns(dtos);

        var result = (await _service.GetVehiclesByUserIdAsync(1)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].UserId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetVehicleByLicensePlateAsync_Exists_ReturnsVehicle()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };
        var dto = new VehicleResponseDto { Id = 1, LicensePlate = "ABC123" };

        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });
        _mockMapper.Setup(m => m.Map<VehicleResponseDto>(vehicle)).Returns(dto);

        var result = await _service.GetVehicleByLicensePlateAsync("ABC123");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.LicensePlate, Is.EqualTo("ABC123"));
    }

    [Test]
    public async Task GetVehicleByLicensePlateAsync_NotFound_ReturnsNull()
    {
        _mockVehicleRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
            .ReturnsAsync(new List<Vehicle>());

        var result = await _service.GetVehicleByLicensePlateAsync("UNKNOWN");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateVehicleAsync_ValidDto_ReturnsCreatedVehicle()
    {
        var createDto = new CreateVehicleDto { LicensePlate = "ABC123", Color = "Red", VehicleType = VehicleType.Car };
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123", Color = "Red", VehicleType = VehicleType.Car };
        var responseDto = new VehicleResponseDto { Id = 1, LicensePlate = "ABC123", Color = "Red" };

        _mockMapper.Setup(m => m.Map<Vehicle>(createDto)).Returns(vehicle);
        _mockMapper.Setup(m => m.Map<VehicleResponseDto>(vehicle)).Returns(responseDto);

        var result = await _service.CreateVehicleAsync(createDto);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.LicensePlate, Is.EqualTo("ABC123"));
        _mockVehicleRepo.Verify(r => r.AddAsync(vehicle), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateVehicleAsync_Exists_ReturnsUpdatedVehicle()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123", Color = "Red" };
        var updateDto = new UpdateVehicleDto { LicensePlate = "ABC123", Color = "Blue", VehicleType = VehicleType.Car };
        var responseDto = new VehicleResponseDto { Id = 1, LicensePlate = "ABC123", Color = "Blue" };

        _mockVehicleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);
        _mockMapper.Setup(m => m.Map(updateDto, vehicle));
        _mockMapper.Setup(m => m.Map<VehicleResponseDto>(vehicle)).Returns(responseDto);

        var result = await _service.UpdateVehicleAsync(1, updateDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo("Blue"));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateVehicleAsync_NotFound_ReturnsNull()
    {
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Vehicle?)null);

        var result = await _service.UpdateVehicleAsync(99, new UpdateVehicleDto());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteVehicleAsync_Exists_ReturnsTrue()
    {
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC123" };
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);

        var result = await _service.DeleteVehicleAsync(1);

        Assert.That(result, Is.True);
        _mockVehicleRepo.Verify(r => r.Remove(vehicle), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteVehicleAsync_NotFound_ReturnsFalse()
    {
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Vehicle?)null);

        var result = await _service.DeleteVehicleAsync(99);

        Assert.That(result, Is.False);
    }
}