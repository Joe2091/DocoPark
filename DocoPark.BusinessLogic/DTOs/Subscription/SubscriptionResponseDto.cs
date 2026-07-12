using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.Subscription;

public sealed record SubscriptionResponseDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public SubscriptionType Type { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public decimal MonthlyFee { get; init; }
    public bool IsActive { get; init; }
    public string UserName { get; init; } = string.Empty;
}