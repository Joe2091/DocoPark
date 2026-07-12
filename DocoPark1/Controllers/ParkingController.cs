using DocoPark.BusinessLogic.DTOs.ParkingSession;
using DocoPark.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ParkingController : ControllerBase
{
    private readonly IParkingSessionService parkingSessionService;
    private readonly ILogger<ParkingController> logger;

    public ParkingController(IParkingSessionService parkingSessionService, ILogger<ParkingController> logger)
    {
        this.parkingSessionService = parkingSessionService;
        this.logger = logger;
    }

    /// <summary>
    /// Check in a vehicle by license plate.
    /// </summary>
    [HttpPost("check-in")]
    [ProducesResponseType(typeof(ParkingSessionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ParkingSessionResponseDto>> CheckIn([FromBody] CheckInRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        logger.LogInformation("Check-in requested for vehicle {LicensePlate}.", dto.LicensePlate);
        var session = await parkingSessionService.CheckInAsync(dto);
        logger.LogInformation("Vehicle {LicensePlate} checked in to spot {SpotNumber}.", dto.LicensePlate, session.SpotNumber);

        return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
    }

    /// <summary>
    /// Check out a vehicle by license plate.
    /// </summary>
    [HttpPost("check-out")]
    [ProducesResponseType(typeof(ParkingSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkingSessionResponseDto>> CheckOut([FromBody] CheckOutRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        logger.LogInformation("Check-out requested for vehicle {LicensePlate}.", dto.LicensePlate);
        var session = await parkingSessionService.CheckOutAsync(dto);
        logger.LogInformation("Vehicle {LicensePlate} checked out. Charge: €{Cost}.", dto.LicensePlate, session.TotalCost);

        return Ok(session);
    }

    /// <summary>
    /// Get all active parking sessions.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ParkingSessionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ParkingSessionResponseDto>>> GetActiveSessions()
    {
        logger.LogInformation("Retrieving all active parking sessions.");
        var sessions = await parkingSessionService.GetActiveSessionsAsync();
        return Ok(sessions);
    }

    /// <summary>
    /// Get a parking session by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ParkingSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkingSessionResponseDto>> GetById(int id)
    {
        var session = await parkingSessionService.GetSessionByIdAsync(id);
        if (session is null)
            return NotFound(new { message = $"Session with ID {id} not found." });

        return Ok(session);
    }
}