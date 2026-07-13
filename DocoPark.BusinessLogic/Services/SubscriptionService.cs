using DocoPark.BusinessLogic.DTOs.Subscription;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Constants;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork unitOfWork;

    public SubscriptionService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionResponseDto> CreateSubscriptionAsync(CreateSubscriptionDto dto)
    {
        var users = await unitOfWork.Users.FindAsync(u => u.Id == dto.UserId);
        var user = users.FirstOrDefault()
            ?? throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");

        // Check if user already has an active subscription
        var existingSubscriptions = await unitOfWork.Subscriptions.FindAsync(
            s => s.UserId == dto.UserId && s.IsActive);
        if (existingSubscriptions.Any())
            throw new InvalidOperationException($"User with ID {dto.UserId} already has an active subscription.");

        // Set fee based on subscription type
        var monthlyFee = dto.Type == SubscriptionType.Monthly ? PricingConstants.MonthlyFee : 0m;

        var subscription = new Subscription
        {
            UserId = user.Id,
            Type = dto.Type,
            StartDate = dto.StartDate,
            EndDate = null,
            MonthlyFee = monthlyFee,
            IsActive = true
        };

        // Monthly users get premium status
        if (dto.Type == SubscriptionType.Monthly)
        {
            user.IsPremium = true;
            user.SubscriptionType = SubscriptionType.Monthly;
            unitOfWork.Users.Update(user);
        }
        else
        {
            user.SubscriptionType = dto.Type;
            unitOfWork.Users.Update(user);
        }

        await unitOfWork.Subscriptions.AddAsync(subscription);
        await unitOfWork.SaveChangesAsync();

        return MapToResponse(subscription, user.Name);
    }

    public async Task<SubscriptionResponseDto?> GetSubscriptionByIdAsync(int id)
    {
        var subscription = await unitOfWork.Subscriptions.GetByIdAsync(id);
        if (subscription is null) return null;

        var users = await unitOfWork.Users.FindAsync(u => u.Id == subscription.UserId);
        var user = users.First();

        return MapToResponse(subscription, user.Name);
    }

    public async Task<IEnumerable<SubscriptionResponseDto>> GetSubscriptionsByUserIdAsync(int userId)
    {
        var subscriptions = await unitOfWork.Subscriptions.FindAsync(s => s.UserId == userId);
        var results = new List<SubscriptionResponseDto>();

        foreach (var subscription in subscriptions)
        {
            var users = await unitOfWork.Users.FindAsync(u => u.Id == subscription.UserId);
            var user = users.First();

            results.Add(MapToResponse(subscription, user.Name));
        }

        return results;
    }

    public async Task<IEnumerable<SubscriptionResponseDto>> GetActiveSubscriptionsAsync()
    {
        var subscriptions = await unitOfWork.Subscriptions.FindAsync(s => s.IsActive);
        var results = new List<SubscriptionResponseDto>();

        foreach (var subscription in subscriptions)
        {
            var users = await unitOfWork.Users.FindAsync(u => u.Id == subscription.UserId);
            var user = users.First();

            results.Add(MapToResponse(subscription, user.Name));
        }

        return results;
    }

    public async Task<bool> CancelSubscriptionAsync(int id)
    {
        var subscription = await unitOfWork.Subscriptions.GetByIdAsync(id);
        if (subscription is null) return false;

        subscription.IsActive = false;
        subscription.EndDate = DateTime.UtcNow;
        unitOfWork.Subscriptions.Update(subscription);

        if (subscription.Type == SubscriptionType.Monthly)
        {
            var users = await unitOfWork.Users.FindAsync(u => u.Id == subscription.UserId);
            var user = users.First();
            user.IsPremium = false;
            unitOfWork.Users.Update(user);
        }

        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private static SubscriptionResponseDto MapToResponse(Subscription subscription, string userName)
    {
        return new SubscriptionResponseDto
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            Type = subscription.Type,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            MonthlyFee = subscription.MonthlyFee,
            IsActive = subscription.IsActive,
            UserName = userName
        };
    }
}