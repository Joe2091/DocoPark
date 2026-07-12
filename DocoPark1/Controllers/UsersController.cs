using DocoPark.BusinessLogic.DTOs.User;
using DocoPark.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocoParkWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<UsersController> logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    /// <summary>
    /// Get all users.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all users.");
        var users = await userService.GetAllUsersAsync();
        logger.LogInformation("Retrieved {Count} users.", users.Count());
        return Ok(users);
    }

    /// <summary>
    /// Get a user by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        logger.LogInformation("Retrieving user with ID {UserId}.", id);
        var user = await userService.GetUserByIdAsync(id);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Create user failed due to invalid model state.");
            return BadRequest(ModelState);
        }

        logger.LogInformation("Creating new user with email {Email}.", dto.Email);
        var user = await userService.CreateUserAsync(dto);
        logger.LogInformation("User created successfully with ID {UserId}.", user.Id);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update an existing user.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Update user {UserId} failed due to invalid model state.", id);
            return BadRequest(ModelState);
        }

        logger.LogInformation("Updating user with ID {UserId}.", id);
        var user = await userService.UpdateUserAsync(id, dto);
        if (user is null)
        {
            logger.LogWarning("Update failed. User with ID {UserId} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        logger.LogInformation("User {UserId} updated successfully.", id);
        return Ok(user);
    }

    /// <summary>
    /// Delete a user by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        logger.LogInformation("Deleting user with ID {UserId}.", id);
        var deleted = await userService.DeleteUserAsync(id);
        if (!deleted)
        {
            logger.LogWarning("Delete failed. User with ID {UserId} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        logger.LogInformation("User {UserId} deleted successfully.", id);
        return NoContent();
    }

    /// <summary>
    /// Get users filtered by subscription type.
    /// </summary>
    [HttpGet("by-subscription/{subscriptionType}")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetBySubscriptionType(string subscriptionType)
    {
        logger.LogInformation("Retrieving users with subscription type {SubscriptionType}.", subscriptionType);
        var users = await userService.GetUsersBySubscriptionTypeAsync(subscriptionType);
        logger.LogInformation("Found {Count} users with subscription type {SubscriptionType}.", users.Count(), subscriptionType);
        return Ok(users);
    }
}