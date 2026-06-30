using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleResponseDto>>> GetAll()
    {
        _logger.LogInformation("Retrieving all vehicles.");
        var vehicles = await _vehicleService.GetAllVehiclesAsync();
        return Ok(vehicles);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Retrieving vehicle with ID {VehicleId}.", id);
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        if (vehicle is null)
        {
            _logger.LogWarning("Vehicle with ID {VehicleId} not found.", id);
            return NotFound(new { message = $"Vehicle with ID {id} not found." });
        }

        return Ok(vehicle);
    }

    [HttpGet("by-user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleResponseDto>>> GetByUserId(int userId)
    {
        _logger.LogInformation("Retrieving vehicles for user {UserId}.", userId);
        var vehicles = await _vehicleService.GetVehiclesByUserIdAsync(userId);
        return Ok(vehicles);
    }

    [HttpGet("by-plate/{licensePlate}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> GetByLicensePlate(string licensePlate)
    {
        _logger.LogInformation("Retrieving vehicle with plate {LicensePlate}.", licensePlate);
        var vehicle = await _vehicleService.GetVehicleByLicensePlateAsync(licensePlate);
        if (vehicle is null)
        {
            _logger.LogWarning("Vehicle with plate {LicensePlate} not found.", licensePlate);
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
            _logger.LogWarning("Create vehicle failed due to invalid model state.");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating vehicle with plate {LicensePlate}.", dto.LicensePlate);
        var vehicle = await _vehicleService.CreateVehicleAsync(dto);
        _logger.LogInformation("Vehicle created with ID {VehicleId}.", vehicle.Id);
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
            _logger.LogWarning("Update vehicle {VehicleId} failed due to invalid model state.", id);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating vehicle with ID {VehicleId}.", id);
        var vehicle = await _vehicleService.UpdateVehicleAsync(id, dto);
        if (vehicle is null)
        {
            _logger.LogWarning("Update failed. Vehicle with ID {VehicleId} not found.", id);
            return NotFound(new { message = $"Vehicle with ID {id} not found." });
        }

        _logger.LogInformation("Vehicle {VehicleId} updated successfully.", id);
        return Ok(vehicle);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting vehicle with ID {VehicleId}.", id);
        var deleted = await _vehicleService.DeleteVehicleAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Vehicle with ID {VehicleId} not found.", id);
            return NotFound(new { message = $"Vehicle with ID {id} not found." });
        }

        _logger.LogInformation("Vehicle {VehicleId} deleted successfully.", id);
        return NoContent();
    }
}