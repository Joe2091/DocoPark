namespace DocoPark.Domain.Entities
{
    public class ParkingSession
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int ParkingSpotId { get; set; }
        public int? UserId { get; set; }
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
        public DateTime? CheckOutTime { get; set; }
        public decimal TotalCost { get; set; }
        public bool IsPaid { get; set; }

        // Navigation properties
        public Vehicle Vehicle { get; set; } = null!;
        public ParkingSpot ParkingSpot { get; set; } = null!;
        public User? User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}