using DocoPark.BusinessLogic.DTOs.User;

namespace DocoPark.BusinessLogic.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<UserResponseDto>> GetUsersBySubscriptionTypeAsync(string subscriptionType);
}