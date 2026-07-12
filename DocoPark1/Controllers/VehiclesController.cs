using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VehiclesController : ControllerBase
{
    private readonly IVehicleService vehicleService;
    private readonly ILogger<VehiclesController> logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        this.vehicleService = vehicleService;
        this.logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleResponseDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all vehicles.");
        var vehicles = await vehicleService.GetAllVehiclesAsync();
        return Ok(vehicles);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> GetById(int id)
    {
        logger.LogInformation("Retrieving vehicle with ID {VehicleId}.", id);
        var vehicle = await vehicleService.GetVehicleByIdAsync(id);
        if (vehicle is null)
        {
            logger.LogWarning("Vehicle with ID {VehicleId} not found.", id);
            return NotFound(new { message = $"Vehicle with ID {id} not found." });
        }

        return Ok(vehicle);
    }

    [HttpGet("by-user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleResponseDto>>> GetByUserId(int userId)
    {
        logger.LogInformation("Retrieving vehicles for user {UserId}.", userId);
        var vehicles = await vehicleService.GetVehiclesByUserIdAsync(userId);
        return Ok(vehicles);
    }

    [HttpGet("by-plate/{licensePlate}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> GetByLicensePlate(string licensePlate)
    {
        logger.LogInformation("Retrieving vehicle with plate {LicensePlate}.", licensePlate);
        var vehicle = await vehicleService.GetVehicleByLicensePlateAsync(licensePlate);
        if (vehicle is null)
        {
            logger.LogWarning("Vehicle with plate {LicensePlate} not found.", licensePlate);
            return NotFound(new { message = $"Vehicle with plate '{licensePlate}' not found." });
        }

        return Ok(vehicle);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleResponseDto>> Create([FromBody] CreateVehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Create vehicle failed due to invalid model state.");
            return BadRequest(ModelState);
        }

        logger.LogInformation("Creating vehicle with plate {LicensePlate}.", dto.LicensePlate);
        var vehicle = await vehicleService.CreateVehicleAsync(dto);
        logger.LogInformation("Vehicle created with ID {VehicleId}.", vehicle.Id);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleResponseDto>> Update(int id, [FromBody] UpdateVehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Update vehicle {VehicleId} failed due to invalid model state.", id);
            return BadRequest(ModelState);
        }

        logger.LogInformation("Updating vehicle with ID {VehicleId}.", id);
        var vehicle = await vehicleService.UpdateVehicleAsync(id, dto);
        if (vehicle is null)
        {
            logger.LogWarning("Update failed. Vehicle with ID {VehicleId} not found.", id);
            return NotFound(new { message = $"Vehicle with ID {id} not found." });
        }

        logger.LogInformation("Vehicle {VehicleId} updated successfully.", id);
        return Ok(vehicle);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        logger.LogInformation("Deleting vehicle with ID {VehicleId}.", id);
        var deleted = await vehicleService.DeleteVehicleAsync(id);
        if (!deleted)
        {
            logger.LogWarning("Delete failed. Vehicle with ID {VehicleId} not found.", id);
            return NotFound(new { message = $"Vehicle with ID {id} not found." });
        }

        logger.LogInformation("Vehicle {VehicleId} deleted successfully.", id);
        return NoContent();
    }
}