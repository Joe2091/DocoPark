using DocoPark.BusinessLogic.DTOs.Vehicle;

namespace DocoPark.BusinessLogic.Interfaces.Services;

public interface IVehicleService
{
    Task<IEnumerable<VehicleResponseDto>> GetAllVehiclesAsync();
    Task<VehicleResponseDto?> GetVehicleByIdAsync(int id);
    Task<IEnumerable<VehicleResponseDto>> GetVehiclesByUserIdAsync(int userId);
    Task<VehicleResponseDto?> GetVehicleByLicensePlateAsync(string licensePlate);
    Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto);
    Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
    Task<bool> DeleteVehicleAsync(int id);
}