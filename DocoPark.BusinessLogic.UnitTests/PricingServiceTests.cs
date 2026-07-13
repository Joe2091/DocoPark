using DocoPark.BusinessLogic.Services;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.UnitTests;

[TestFixture]
public class PricingServiceTests
{
    private PricingService _pricingService;

    [SetUp]
    public void SetUp()
    {
        _pricingService = new PricingService();
    }

    [Test]
    public void CalculateSessionCost_MonthlySubscription_ReturnsZero()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 17, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Monthly);

        Assert.That(result, Is.EqualTo(0m));
    }

    [Test]
    public void CalculateSessionCost_DailySubscription_ReturnsDailyRate()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Daily);

        Assert.That(result, Is.EqualTo(10.00m));
    }

    [Test]
    public void CalculateSessionCost_Hourly_OneHour_Returns3()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(3.00m));
    }

    [Test]
    public void CalculateSessionCost_Hourly_ThreeHours_Returns9()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 11, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(9.00m));
    }

    [Test]
    public void CalculateSessionCost_Hourly_FourHours_HitsDailyCap_Returns10()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(10.00m));
    }

    [Test]
    public void CalculateSessionCost_Hourly_FullDay_CappedAt10()
    {
        var checkIn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 23, 59, 59, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(10.00m));
    }

    [Test]
    public void CalculateSessionCost_Hourly_TwoDays_Returns20()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 3, 8, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(20.00m));
    }

    [Test]
    public void CalculateSessionCost_Hourly_PartialHour_RoundsUp()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 9, 30, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(6.00m)); // Ceiling(1.5) = 2 hours × €3
    }

    [Test]
    public void CalculateSessionCost_Hourly_ShortStay_15Minutes_Returns3()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 1, 8, 15, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        Assert.That(result, Is.EqualTo(3.00m)); // Ceiling(0.25) = 1 hour × €3
    }

    [Test]
    public void CalculateSessionCost_Hourly_MultiDay_PartialSecondDay_Returns16()
    {
        var checkIn = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var checkOut = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);

        var result = _pricingService.CalculateSessionCost(checkIn, checkOut, SubscriptionType.Hourly);

        // Day 1: 16 hours → capped at €10
        // Day 2: 2 hours → 2 × €3 = €6
        Assert.That(result, Is.EqualTo(16.00m));
    }
}