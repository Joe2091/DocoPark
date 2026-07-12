using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.User;

public sealed record UserResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public SubscriptionType SubscriptionType { get; init; }
    public bool IsPremium { get; init; }
    public DateTime CreatedDate { get; init; }
    public int VehicleCount { get; init; }
}