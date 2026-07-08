using System.ComponentModel.DataAnnotations;

namespace DocoPark.BusinessLogic.DTOs.ParkingSession;

public sealed class CheckOutRequestDto
{
    [Required]
    public string LicensePlate { get; set; } = string.Empty;
}