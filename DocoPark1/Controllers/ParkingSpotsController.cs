using DocoPark.BusinessLogic.Interfaces;
using DocoPark.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ParkingSpotsController : ControllerBase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<ParkingSpotsController> logger;

    public ParkingSpotsController(IUnitOfWork unitOfWork, ILogger<ParkingSpotsController> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    /// <summary>
    /// Get all parking spots with their current status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Retrieving all parking spots.");
        var spots = await unitOfWork.ParkingSpots.GetAllAsync();
        return Ok(spots.Select(s => new
        {
            s.Id,
            s.SpotNumber,
            Status = s.SpotStatus.ToString(),
            s.CurrentSessionId
        }));
    }

    /// <summary>
    /// Get all available parking spots.
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailable()
    {
        logger.LogInformation("Retrieving available parking spots.");
        var spots = await unitOfWork.ParkingSpots.FindAsync(s => s.SpotStatus == SpotStatus.Available);
        return Ok(spots.Select(s => new
        {
            s.Id,
            s.SpotNumber
        }));
    }

    /// <summary>
    /// Get parking spot occupancy summary.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        logger.LogInformation("Retrieving parking spot summary.");
        var allSpots = await unitOfWork.ParkingSpots.GetAllAsync();
        var spotList = allSpots.ToList();

        return Ok(new
        {
            Total = spotList.Count,
            Available = spotList.Count(s => s.SpotStatus == SpotStatus.Available),
            Occupied = spotList.Count(s => s.SpotStatus == SpotStatus.Occupied),
            Reserved = spotList.Count(s => s.SpotStatus == SpotStatus.Reserved)
        });
    }

    /// <summary>
    /// Get a specific parking spot by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var spot = await unitOfWork.ParkingSpots.GetByIdAsync(id);
        if (spot is null)
            return NotFound(new { message = $"Parking spot with ID {id} not found." });

        return Ok(new
        {
            spot.Id,
            spot.SpotNumber,
            Status = spot.SpotStatus.ToString(),
            spot.CurrentSessionId
        });
    }
}