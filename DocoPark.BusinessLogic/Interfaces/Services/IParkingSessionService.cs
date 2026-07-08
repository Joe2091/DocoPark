using DocoPark.BusinessLogic.DTOs.ParkingSession;

namespace DocoPark.BusinessLogic.Interfaces.Services;

public interface IParkingSessionService
{
    Task<ParkingSessionResponseDto> CheckInAsync(CheckInRequestDto dto);
    Task<ParkingSessionResponseDto> CheckOutAsync(CheckOutRequestDto dto);
    Task<IEnumerable<ParkingSessionResponseDto>> GetActiveSessionsAsync();
    Task<ParkingSessionResponseDto?> GetSessionByIdAsync(int id);
}