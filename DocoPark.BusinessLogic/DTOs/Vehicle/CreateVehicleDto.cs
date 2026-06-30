using System.ComponentModel.DataAnnotations;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.Vehicle;

public sealed class CreateVehicleDto
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Color { get; set; } = string.Empty;

    [Required]
    public VehicleType VehicleType { get; set; }

    public int? UserId { get; set; }
}