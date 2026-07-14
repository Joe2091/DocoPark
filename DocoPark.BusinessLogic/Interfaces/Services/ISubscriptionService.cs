using DocoPark.BusinessLogic.DTOs.Subscription;

namespace DocoPark.BusinessLogic.Interfaces.Services;

public interface ISubscriptionService
{
    Task<SubscriptionResponseDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
    Task<SubscriptionResponseDto?> GetSubscriptionByIdAsync(int id);
    Task<IEnumerable<SubscriptionResponseDto>> GetAllSubscriptionsAsync();
    Task<IEnumerable<SubscriptionResponseDto>> GetSubscriptionsByUserIdAsync(int userId);
    Task<IEnumerable<SubscriptionResponseDto>> GetActiveSubscriptionsAsync();
    Task<bool> CancelSubscriptionAsync(int id);
    Task<bool> DeleteSubscriptionAsync(int id);
}
