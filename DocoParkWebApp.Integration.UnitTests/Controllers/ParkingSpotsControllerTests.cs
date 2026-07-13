using System.Linq.Expressions;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Controllers;

[TestFixture]
public class ParkingSpotsControllerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IParkingSpotRepository> _mockSpotRepo;
    private ParkingSpotsController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSpotRepo = new Mock<IParkingSpotRepository>();
        _mockUnitOfWork.Setup(u => u.ParkingSpots).Returns(_mockSpotRepo.Object);

        var logger = new Mock<ILogger<ParkingSpotsController>>().Object;
        _controller = new ParkingSpotsController(_mockUnitOfWork.Object, logger);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithAllSpots()
    {
        // Arrange
        var spots = new List<ParkingSpot>
        {
            new() { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Available, CurrentSessionId = null },
            new() { Id = 2, SpotNumber = "A2", SpotStatus = SpotStatus.Occupied, CurrentSessionId = 5 },
            new() { Id = 3, SpotNumber = "A3", SpotStatus = SpotStatus.Reserved, CurrentSessionId = null }
        };
        _mockSpotRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        _mockSpotRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ParkingSpot>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
    }

    [Test]
    public async Task GetAvailable_ReturnsOkResult_WithAvailableSpots()
    {
        // Arrange
        var availableSpots = new List<ParkingSpot>
        {
            new() { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Available },
            new() { Id = 3, SpotNumber = "A3", SpotStatus = SpotStatus.Available }
        };
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(availableSpots);

        // Act
        var result = await _controller.GetAvailable();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetAvailable_ReturnsOkResult_WithEmptyList_WhenNoSpotsAvailable()
    {
        // Arrange
        _mockSpotRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ParkingSpot, bool>>>()))
            .ReturnsAsync(new List<ParkingSpot>());

        // Act
        var result = await _controller.GetAvailable();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
    }

    [Test]
    public async Task GetSummary_ReturnsOkResult_WithCorrectCounts()
    {
        // Arrange
        var spots = new List<ParkingSpot>
        {
            new() { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Available },
            new() { Id = 2, SpotNumber = "A2", SpotStatus = SpotStatus.Available },
            new() { Id = 3, SpotNumber = "A3", SpotStatus = SpotStatus.Occupied },
            new() { Id = 4, SpotNumber = "A4", SpotStatus = SpotStatus.Reserved }
        };
        _mockSpotRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

        // Act
        var result = await _controller.GetSummary();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        // Use reflection to verify the anonymous object properties
        var value = okResult!.Value!;
        var total = (int)value.GetType().GetProperty("Total")!.GetValue(value)!;
        var available = (int)value.GetType().GetProperty("Available")!.GetValue(value)!;
        var occupied = (int)value.GetType().GetProperty("Occupied")!.GetValue(value)!;
        var reserved = (int)value.GetType().GetProperty("Reserved")!.GetValue(value)!;

        Assert.That(total, Is.EqualTo(4));
        Assert.That(available, Is.EqualTo(2));
        Assert.That(occupied, Is.EqualTo(1));
        Assert.That(reserved, Is.EqualTo(1));
    }

    [Test]
    public async Task GetSummary_ReturnsAllZeros_WhenNoSpots()
    {
        // Arrange
        _mockSpotRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ParkingSpot>());

        // Act
        var result = await _controller.GetSummary();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var value = okResult!.Value!;
        var total = (int)value.GetType().GetProperty("Total")!.GetValue(value)!;
        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenSpotExists()
    {
        // Arrange
        var spot = new ParkingSpot { Id = 1, SpotNumber = "A1", SpotStatus = SpotStatus.Available, CurrentSessionId = null };
        _mockSpotRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(spot);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenSpotDoesNotExist()
    {
        // Arrange
        _mockSpotRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ParkingSpot?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
    }
}