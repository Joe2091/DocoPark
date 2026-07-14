using DocoPark.BusinessLogic.DTOs.Subscription;
using DocoPark.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService subscriptionService;
    private readonly ILogger<SubscriptionsController> logger;

    public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
    {
        this.subscriptionService = subscriptionService;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new subscription.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubscriptionResponseDto>> Create([FromBody] CreateSubscriptionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        logger.LogInformation("Creating subscription for user {UserId} with type {Type}.", dto.UserId, dto.Type);
        var subscription = await subscriptionService.CreateSubscriptionAsync(dto);
        logger.LogInformation("Subscription {SubscriptionId} created successfully.", subscription.Id);

        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    /// <summary>
    /// Get a subscription by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionResponseDto>> GetById(int id)
    {
        var subscription = await subscriptionService.GetSubscriptionByIdAsync(id);
        if (subscription is null)
        {
            logger.LogWarning("Subscription with ID {SubscriptionId} not found.", id);
            return NotFound(new { message = $"Subscription with ID {id} not found." });
        }

        return Ok(subscription);
    }

    /// <summary>
    /// Get all subscriptions for a user.
    /// </summary>
    [HttpGet("by-user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionResponseDto>>> GetByUserId(int userId)
    {
        logger.LogInformation("Retrieving subscriptions for user {UserId}.", userId);
        var subscriptions = await subscriptionService.GetSubscriptionsByUserIdAsync(userId);
        return Ok(subscriptions);
    }

    /// <summary>
    /// Get all active subscriptions.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionResponseDto>>> GetActive()
    {
        logger.LogInformation("Retrieving all active subscriptions.");
        var subscriptions = await subscriptionService.GetActiveSubscriptionsAsync();
        return Ok(subscriptions);
    }

    /// <summary>
    /// Get all subscriptions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionResponseDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all subscriptions.");
        var subscriptions = await subscriptionService.GetAllSubscriptionsAsync();
        return Ok(subscriptions);
    }

    /// <summary>
    /// Cancel a subscription.
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id)
    {
        logger.LogInformation("Cancelling subscription {SubscriptionId}.", id);
        var cancelled = await subscriptionService.CancelSubscriptionAsync(id);
        if (!cancelled)
        {
            logger.LogWarning("Cancel failed. Subscription with ID {SubscriptionId} not found.", id);
            return NotFound(new { message = $"Subscription with ID {id} not found." });
        }

        logger.LogInformation("Subscription {SubscriptionId} cancelled successfully.", id);
        return NoContent();
    }

    /// <summary>
    /// Delete a subscription.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        logger.LogInformation("Deleting subscription {SubscriptionId}.", id);
        var deleted = await subscriptionService.DeleteSubscriptionAsync(id);
        if (!deleted)
        {
            logger.LogWarning("Delete failed. Subscription with ID {SubscriptionId} not found.", id);
            return NotFound(new { message = $"Subscription with ID {id} not found." });
        }

        logger.LogInformation("Subscription {SubscriptionId} deleted successfully.", id);
        return NoContent();
    }
}