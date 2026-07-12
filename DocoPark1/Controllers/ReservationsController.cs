using DocoPark.BusinessLogic.DTOs.Reservation;
using DocoPark.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IReservationService reservationService;
    private readonly ILogger<ReservationsController> logger;

    public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger)
    {
        this.reservationService = reservationService;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new reservation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservationResponseDto>> Create([FromBody] CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        logger.LogInformation("Creating reservation for user {UserId} on spot {SpotId}.", dto.UserId, dto.ParkingSpotId);
        var reservation = await reservationService.CreateReservationAsync(dto);
        logger.LogInformation("Reservation {ReservationId} created successfully.", reservation.Id);

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    /// <summary>
    /// Get a reservation by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReservationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationResponseDto>> GetById(int id)
    {
        var reservation = await reservationService.GetReservationsByIdAsync(id);
        if (reservation is null)
        {
            logger.LogWarning("Reservation with ID {ReservationId} not found.", id);
            return NotFound(new { message = $"Reservation with ID {id} not found." });
        }

        return Ok(reservation);
    }

    /// <summary>
    /// Get all active reservations.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ReservationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservationResponseDto>>> GetActive()
    {
        logger.LogInformation("Retrieving all active reservations.");
        var reservations = await reservationService.GetActiveReservationsAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Get all reservations for a user.
    /// </summary>
    [HttpGet("by-user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ReservationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservationResponseDto>>> GetByUserId(int userId)
    {
        logger.LogInformation("Retrieving reservations for user {UserId}.", userId);
        var reservations = await reservationService.GetReservationByUserIdAsync(userId);
        return Ok(reservations);
    }

    /// <summary>
    /// Cancel a reservation.
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id)
    {
        logger.LogInformation("Cancelling reservation {ReservationId}.", id);
        var cancelled = await reservationService.CancelReservationAsync(id);
        if (!cancelled)
        {
            logger.LogWarning("Cancel failed. Reservation with ID {ReservationId} not found.", id);
            return NotFound(new { message = $"Reservation with ID {id} not found." });
        }

        logger.LogInformation("Reservation {ReservationId} cancelled successfully.", id);
        return NoContent();
    }
}
