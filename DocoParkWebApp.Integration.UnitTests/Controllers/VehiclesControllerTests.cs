using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Enums;
using DocoParkWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocoParkWebApp.Integration.UnitTests.Controllers;

[TestFixture]
public class VehiclesControllerTests
{
    private Mock<IVehicleService> _mockVehicleService;
    private VehiclesController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockVehicleService = new Mock<IVehicleService>();
        var logger = new Mock<ILogger<VehiclesController>>().Object;
        _controller = new VehiclesController(_mockVehicleService.Object, logger);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfVehicles()
    {
        // Arrange
        var vehicles = new List<VehicleResponseDto>
        {
            new() { Id = 1, LicensePlate = "21-D-1234", Color = "Red", VehicleType = VehicleType.Car },
            new() { Id = 2, LicensePlate = "22-G-5678", Color = "Blue", VehicleType = VehicleType.Van }
        };
        _mockVehicleService.Setup(s => s.GetAllVehiclesAsync()).ReturnsAsync(vehicles);

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedVehicles = okResult!.Value as IEnumerable<VehicleResponseDto>;
        Assert.That(returnedVehicles, Is.Not.Null);
        Assert.That(returnedVehicles!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenVehicleExists()
    {
        // Arrange
        var vehicle = new VehicleResponseDto { Id = 1, LicensePlate = "21-D-1234", Color = "Red" };
        _mockVehicleService.Setup(s => s.GetVehicleByIdAsync(1)).ReturnsAsync(vehicle);

        // Act
        var actionResult = await _controller.GetById(1);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedVehicle = okResult!.Value as VehicleResponseDto;
        Assert.That(returnedVehicle, Is.Not.Null);
        Assert.That(returnedVehicle!.LicensePlate, Is.EqualTo("21-D-1234"));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenVehicleDoesNotExist()
    {
        // Arrange
        _mockVehicleService.Setup(s => s.GetVehicleByIdAsync(99)).ReturnsAsync((VehicleResponseDto?)null);

        // Act
        var actionResult = await _controller.GetById(99);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task Create_ReturnsCreatedResult_WhenValid()
    {
        // Arrange
        var dto = new CreateVehicleDto { LicensePlate = "21-D-1234", Color = "Red", VehicleType = VehicleType.Car };
        var response = new VehicleResponseDto { Id = 1, LicensePlate = "21-D-1234", Color = "Red", VehicleType = VehicleType.Car };
        _mockVehicleService.Setup(s => s.CreateVehicleAsync(dto)).ReturnsAsync(response);

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var createdResult = actionResult.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task Delete_ReturnsNoContent_WhenVehicleExists()
    {
        // Arrange
        _mockVehicleService.Setup(s => s.DeleteVehicleAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenVehicleDoesNotExist()
    {
        // Arrange
        _mockVehicleService.Setup(s => s.DeleteVehicleAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}