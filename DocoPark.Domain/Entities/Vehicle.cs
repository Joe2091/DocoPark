using DocoPark.Domain.Enums;

namespace DocoPark.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
        public int? UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}