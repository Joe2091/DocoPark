using DocoPark.Domain.Enums;

namespace DocoPark.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ParkingSpotId { get; set; }
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
        public int VehicleId { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public ParkingSpot ParkingSpot { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
    }
}