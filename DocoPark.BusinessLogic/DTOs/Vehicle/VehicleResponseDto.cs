using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.Vehicle;

public sealed class VehicleResponseDto
{
    public int Id { get; init; }
    public string LicensePlate { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public VehicleType VehicleType { get; init; }
    public int? UserId { get; init; }
    public string? OwnerName { get; init; }
}