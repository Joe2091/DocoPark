using DocoPark.BusinessLogic.DTOs.ParkingSession;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Enums;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Controllers;

[TestFixture]
public class ParkingControllerTests
{
    private Mock<IParkingSessionService> _mockParkingService;
    private ParkingController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockParkingService = new Mock<IParkingSessionService>();
        var logger = new Mock<ILogger<ParkingController>>().Object;
        _controller = new ParkingController(_mockParkingService.Object, logger);
    }

    [Test]
    public async Task CheckIn_ReturnsCreatedResult_WhenSuccessful()
    {
        // Arrange
        var dto = new CheckInRequestDto { LicensePlate = "21-D-1234" };
        var response = new ParkingSessionResponseDto
        {
            Id = 1,
            LicensePlate = "21-D-1234",
            SpotNumber = "S001",
            CheckInTime = DateTime.UtcNow,
            SpotStatus = SpotStatus.Occupied
        };
        _mockParkingService.Setup(s => s.CheckInAsync(dto)).ReturnsAsync(response);

        // Act
        var actionResult = await _controller.CheckIn(dto);

        // Assert
        var createdResult = actionResult.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task CheckOut_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var dto = new CheckOutRequestDto { LicensePlate = "21-D-1234" };
        var response = new ParkingSessionResponseDto
        {
            Id = 1,
            LicensePlate = "21-D-1234",
            SpotNumber = "S001",
            CheckInTime = DateTime.UtcNow.AddHours(-2),
            CheckOutTime = DateTime.UtcNow,
            TotalCost = 6.00m,
            SpotStatus = SpotStatus.Available
        };
        _mockParkingService.Setup(s => s.CheckOutAsync(dto)).ReturnsAsync(response);

        // Act
        var actionResult = await _controller.CheckOut(dto);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var session = okResult!.Value as ParkingSessionResponseDto;
        Assert.That(session, Is.Not.Null);
        Assert.That(session!.TotalCost, Is.EqualTo(6.00m));
    }

    [Test]
    public async Task GetActiveSessions_ReturnsOkResult_WithSessions()
    {
        // Arrange
        var sessions = new List<ParkingSessionResponseDto>
        {
            new() { Id = 1, LicensePlate = "21-D-1234", SpotNumber = "S001", SpotStatus = SpotStatus.Occupied },
            new() { Id = 2, LicensePlate = "22-G-5678", SpotNumber = "S002", SpotStatus = SpotStatus.Occupied }
        };
        _mockParkingService.Setup(s => s.GetActiveSessionsAsync()).ReturnsAsync(sessions);

        // Act
        var actionResult = await _controller.GetActiveSessions();

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedSessions = okResult!.Value as IEnumerable<ParkingSessionResponseDto>;
        Assert.That(returnedSessions!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenSessionDoesNotExist()
    {
        // Arrange
        _mockParkingService.Setup(s => s.GetSessionByIdAsync(99)).ReturnsAsync((ParkingSessionResponseDto?)null);

        // Act
        var actionResult = await _controller.GetById(99);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }
}