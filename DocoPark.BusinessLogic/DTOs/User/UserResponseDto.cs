using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public SubscriptionType SubscriptionType { get; set; }
    public bool IsPremium { get; set; }
    public DateTime CreatedDate { get; set; }
    public int VehicleCount { get; set; }
}