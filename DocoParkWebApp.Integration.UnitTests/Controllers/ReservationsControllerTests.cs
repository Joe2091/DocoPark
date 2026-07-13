using DocoPark.BusinessLogic.DTOs.Reservation;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Enums;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Controllers;

[TestFixture]
public class ReservationsControllerTests
{
    private Mock<IReservationService> _mockReservationService;
    private ReservationsController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockReservationService = new Mock<IReservationService>();
        var logger = new Mock<ILogger<ReservationsController>>().Object;
        _controller = new ReservationsController(_mockReservationService.Object, logger);
    }

    [Test]
    public async Task Create_ReturnsCreatedResult_WhenValid()
    {
        // Arrange
        var dto = new CreateReservationDto
        {
            UserId = 1,
            VehicleId = 1,
            ParkingSpotId = 5,
            ReservedFrom = DateTime.UtcNow.AddHours(1),
            ReservedTo = DateTime.UtcNow.AddHours(3)
        };
        var response = new ReservationResponseDto
        {
            Id = 1,
            UserId = 1,
            VehicleId = 1,
            ParkingSpotId = 5,
            ReservedFrom = dto.ReservedFrom,
            ReservedTo = dto.ReservedTo,
            Status = ReservationStatus.Active,
            SpotNumber = "S005",
            LicensePlate = "21-D-1234"
        };
        _mockReservationService.Setup(s => s.CreateReservationAsync(dto)).ReturnsAsync(response);

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var createdResult = actionResult.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenReservationExists()
    {
        // Arrange
        var reservation = new ReservationResponseDto
        {
            Id = 1,
            UserId = 1,
            Status = ReservationStatus.Active,
            SpotNumber = "S005",
            LicensePlate = "21-D-1234"
        };
        _mockReservationService.Setup(s => s.GetReservationsByIdAsync(1)).ReturnsAsync(reservation);

        // Act
        var actionResult = await _controller.GetById(1);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenReservationDoesNotExist()
    {
        // Arrange
        _mockReservationService.Setup(s => s.GetReservationsByIdAsync(99)).ReturnsAsync((ReservationResponseDto?)null);

        // Act
        var actionResult = await _controller.GetById(99);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task Cancel_ReturnsNoContent_WhenReservationExists()
    {
        // Arrange
        _mockReservationService.Setup(s => s.CancelReservationAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Cancel(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Cancel_ReturnsNotFound_WhenReservationDoesNotExist()
    {
        // Arrange
        _mockReservationService.Setup(s => s.CancelReservationAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Cancel(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetActive_ReturnsOkResult_WithActiveReservations()
    {
        // Arrange
        var reservations = new List<ReservationResponseDto>
        {
            new() { Id = 1, Status = ReservationStatus.Active, SpotNumber = "S001", LicensePlate = "21-D-1234" }
        };
        _mockReservationService.Setup(s => s.GetActiveReservationsAsync()).ReturnsAsync(reservations);

        // Act
        var actionResult = await _controller.GetActive();

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returned = okResult!.Value as IEnumerable<ReservationResponseDto>;
        Assert.That(returned!.Count(), Is.EqualTo(1));
    }
}