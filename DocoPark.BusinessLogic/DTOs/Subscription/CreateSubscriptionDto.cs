using System.ComponentModel.DataAnnotations;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.Subscription;

public sealed class CreateSubscriptionDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public SubscriptionType Type { get; set; }

    [Required]
    public DateTime StartDate { get; set; }
}