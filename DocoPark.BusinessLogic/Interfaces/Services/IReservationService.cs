using DocoPark.BusinessLogic.DTOs.Reservation;

namespace DocoPark.BusinessLogic.Interfaces.Services
    
{
    public interface IReservationService
    {
        Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto);
        Task<ReservationResponseDto?> GetReservationsByIdAsync(int id);
        Task<IEnumerable<ReservationResponseDto>> GetAllReservationsAsync();
        Task<IEnumerable<ReservationResponseDto>> GetActiveReservationsAsync();
        Task<IEnumerable<ReservationResponseDto?>> GetReservationByUserIdAsync(int userId);
        Task<bool> CancelReservationAsync(int id);
    }
}
