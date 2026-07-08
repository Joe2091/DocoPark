using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.Interfaces.Services;

public interface IPricingService
{
    decimal CalculateSessionCost(DateTime checkIn, DateTime checkOut, SubscriptionType subscriptionType);
}