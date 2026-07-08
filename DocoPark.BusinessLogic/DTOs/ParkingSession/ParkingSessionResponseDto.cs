using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.ParkingSession;

public sealed class ParkingSessionResponseDto
{
    public int Id { get; init; }
    public string LicensePlate { get; init; } = string.Empty;
    public string SpotNumber { get; init; } = string.Empty;
    public DateTime CheckInTime { get; init; }
    public DateTime? CheckOutTime { get; init; }
    public decimal TotalCost { get; init; }
    public bool IsPaid { get; init; }
    public SpotStatus SpotStatus { get; init; }
}