using DocoPark.Domain.Enums;

namespace DocoPark.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ParkingSessionId { get; set; }
        public int? SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ParkingSession? ParkingSession { get; set; }
        public Subscription? Subscription { get; set; }
    }
}