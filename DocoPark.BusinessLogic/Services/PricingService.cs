using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Constants;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.Services;

public sealed class PricingService : IPricingService
{
    public decimal CalculateSessionCost(DateTime checkIn, DateTime checkOut, SubscriptionType subscriptionType)
    {
        if (subscriptionType == SubscriptionType.Monthly)
            return 0m;
        var duration = checkOut - checkIn;
        var totalHours = (decimal)Math.Ceiling(duration.TotalHours);

        if (subscriptionType == SubscriptionType.Daily)
            return PricingConstants.DailyRate;

        var totalDays = (int)Math.Ceiling(duration.TotalDays);
        if (totalDays <= 0) totalDays = 1;

        decimal totalCost = 0m;

        for (int day = 0; day <totalDays; day++)
        {
            var dayStart = checkIn.AddDays(day);
            var dayEnd = day == totalDays - 1 ? checkOut : checkIn.AddDays(day + 1);
            var hoursThisDay = (decimal)Math.Ceiling((dayEnd - dayStart).TotalHours);

            var dayCost = hoursThisDay * PricingConstants.HourlyRate;
            dayCost = Math.Min(dayCost, PricingConstants.DailyCap);

            totalCost += dayCost;
        }

        return totalCost;
    }
}