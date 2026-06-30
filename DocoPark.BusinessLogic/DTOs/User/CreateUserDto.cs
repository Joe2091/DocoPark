using System.ComponentModel.DataAnnotations;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.User;

public sealed class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public SubscriptionType SubscriptionType { get; set; }
}